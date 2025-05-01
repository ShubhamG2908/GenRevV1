using Dapper;

using Genrev.Data.DTOs;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Genrev.Web.App.Services
{
    public class CRMService
    {
        private readonly string _connectionString;

        public CRMService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int SaveCRMEntry(CRMViewModel model)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO CRM (SalesPersonId, CustomerId, Name, Title, AreaOfResponsibilityId, 
                                     LinkedInUrl, Concern, CreatedAt, UpdatedAt) 
                    VALUES (@SalesPersonId, @CustomerId, @Name, @Title, @AreaOfResponsibilityId, 
                            @LinkedInUrl, @Concern, GETDATE(), NULL);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                id = connection.ExecuteScalar<int>(query, new
                {
                    model.SalesPersonId,
                    model.CustomerId,
                    model.Name,
                    model.Title,
                    model.AreaOfResponsibilityId,
                    model.LinkedInUrl,
                    Concern = model.Concern ?? (object)DBNull.Value
                });
            }
            return id;
        }
        // Update CRM Entry
        public bool UpdateCRMEntry(CRMViewModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Update the main CRM record
                string query = @"
                    UPDATE CRM 
                    SET SalesPersonId = @SalesPersonId, 
                        CustomerId = @CustomerId, 
                        Name = @Name, 
                        Title = @Title, 
                        AreaOfResponsibilityId = @AreaOfResponsibilityId, 
                        LinkedInUrl = @LinkedInUrl,                                                 
                        Concern = @Concern, 
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id;";

                connection.Execute(query, new
                {
                    model.Id,
                    model.SalesPersonId,
                    model.CustomerId,
                    model.Name,
                    model.Title,
                    model.AreaOfResponsibilityId,
                    model.LinkedInUrl,
                    Concern = model.Concern ?? (object)DBNull.Value
                });

                // Remove existing contacts & addresses
                connection.Execute("DELETE FROM CRM_Contacts WHERE CRMId = @CRMId", new { CRMId = model.Id });
                connection.Execute("DELETE FROM CRM_Address WHERE CRMId = @CRMId", new { CRMId = model.Id });

                // Insert updated contacts
                if (model.Contacts.Count > 0)
                {
                    SaveContacts(model.Id, model.Contacts);
                }

                // Insert updated addresses
                if (model.Addresses.Count > 0)
                {
                    SaveAddresses(model.Id, model.Addresses);
                }
                return true;
            }
        }
        public void SaveContacts(int crmId, List<CRMContactViewModel> contacts)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO CRM_Contacts (CRMId, ContactType, ContactValue, CreatedAt) 
                    VALUES (@CRMId, @ContactType, @ContactValue, GETDATE());";

                foreach (var contact in contacts)
                {
                    if (!string.IsNullOrWhiteSpace(contact.ContactType) && !string.IsNullOrWhiteSpace(contact.ContactValue))
                    {
                        connection.Execute(query, new
                        {
                            CRMId = crmId,
                            contact.ContactType, // 'Phone', 'Email', 'Notes'
                            contact.ContactValue
                        });
                    }
                }
            }
        }

        public void SaveAddresses(int crmId, List<AddressViewModel> addresses)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO CRM_Address (CRMId, AddressLine1, AddressLine2, City, State, Pincode, 
                                             Country, CreatedAt, UpdatedAt) 
                    VALUES (@CRMId, @AddressLine1, @AddressLine2, @City, @State, @Pincode, 
                            @Country, GETDATE(), NULL);";

                foreach (var address in addresses)
                {
                    if (!string.IsNullOrWhiteSpace(address.AddressLine1))
                    {
                        connection.Execute(query, new
                        {
                            CRMId = crmId,
                            address.AddressLine1,
                            address.AddressLine2,
                            address.City,
                            address.State,
                            address.Pincode,
                            address.Country
                        });
                    }
                }
            }
        }
        public List<CRMRecordDTO> GetCRMRecords()
        {
            List<int> personnelIds = AppService.Current.ViewContext.PersonnelIDs.ToList();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                 SELECT 
                 c.Id, c.SalesPersonId, c.CustomerId, c.Name, c.Title, c.AreaOfResponsibilityId, aor.[Name] as AreaOfResponsibilityName, 
                 c.LinkedInUrl, c.Strategy, c.StrategyDate,c.Concern, c.CreatedAt, c.UpdatedAt,
                 (SELECT TOP 1 ContactValue FROM CRM_Contacts WHERE CRMId = c.Id AND ContactType = 'Phone' ORDER BY Id) AS Phone,
                 (SELECT TOP 1 ContactValue FROM CRM_Contacts WHERE CRMId = c.Id AND ContactType = 'Email' ORDER BY Id) AS Email,
                 (SELECT TOP 1 ContactValue FROM CRM_Contacts WHERE CRMId = c.Id AND ContactType = 'Notes' ORDER BY Id) AS Notes,
                 (SELECT TOP 1 CONCAT(AddressLine1, ', ', AddressLine2, ', ', City, ', ', State, ', ', Pincode, ', ', Country) 
                 FROM CRM_Address WHERE CRMId = c.Id ORDER BY Id) AS FirstAddress,
                 (SELECT TOP 1 CONCAT(p.PersonFirstName, ' ', p.PersonLastName) FROM Personnel p WHERE p.Id = c.SalesPersonId) AS SalesPersonName,
                 (SELECT TOP 1 cc.CustomerName FROM CompanyCustomers cc WHERE cc.Id = c.CustomerId) AS CustomerName,
                 (SELECT TOP 1 i.IndustryName FROM CompanyIndustries i WHERE i.Id = cc.CustomerIndustryID) AS IndustryName
                 FROM CRM c
                 JOIN CompanyCustomers cc ON c.CustomerId = cc.Id
                 LEFT JOIN CompanyAreaOfResponsibilities aor ON c.AreaOfResponsibilityId = aor.Id
                 WHERE SalesPersonId IN (" + string.Join(",", personnelIds) + ")";

                var result = connection.Query<CRMRecordDTO>(query).ToList();
                return result;
            }
        }
        public List<CRMRecordDTO> GetCRMRecordsBySalesPersonIdAndCustomerId(int salesPersonId, int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM CRM WHERE SalesPersonId = @SalesPersonId AND CustomerId = @CustomerId";

                return connection.Query<CRMRecordDTO>(query, new
                {
                    SalesPersonId = salesPersonId,
                    CustomerId = customerId
                }).ToList();
            }
        }

        public CRMViewModel GetCRMDetailsById(int crmId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Query to fetch the main CRM record
                string crmQuery = @"
                                                    SELECT c.Id, c.SalesPersonId, c.CustomerId, c.[Name], c.Title, c.AreaOfResponsibilityId, aor.[Name] as AreaOfResponsibilityName,
                                               c.LinkedInUrl, c.Strategy, c.StrategyDate, c.Concern, c.CreatedAt, c.UpdatedAt
                                        FROM CRM c
                                        LEFT JOIN CompanyAreaOfResponsibilities aor ON c.AreaOfResponsibilityId = aor.Id
                                        WHERE c.Id = @CRMId;";

                // Query to fetch all contact details for this CRM record
                string contactsQuery = @"
            SELECT CRMId, ContactType, ContactValue
            FROM CRM_Contacts 
            WHERE CRMId = @CRMId;";

                // Query to fetch all addresses for this CRM record
                string addressQuery = @"
            SELECT AddressLine1, AddressLine2, City, State, Pincode, Country
            FROM CRM_Address 
            WHERE CRMId = @CRMId;";

                var crm = connection.QuerySingleOrDefault<CRMViewModel>(crmQuery, new { CRMId = crmId });
                if (crm == null)
                {
                    return null; // No record found
                }

                // Fetch and map contacts
                var contacts = connection.Query<CRMContactViewModel>(contactsQuery, new { CRMId = crmId }).ToList();
                crm.Contacts = contacts;

                // Fetch and map addresses
                var addresses = connection.Query<AddressViewModel>(addressQuery, new { CRMId = crmId }).ToList();
                crm.Addresses = addresses;

                return crm;
            }
        }
        public string GetStrategyByCustomerId(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT TOP 1 Strategy 
            FROM CustomerData 
            WHERE CustomerID = @CustomerID AND Strategy IS NOT NULL AND LTRIM(RTRIM(Strategy)) <> ''
            ORDER BY ID DESC;";

                return connection.QueryFirstOrDefault<string>(query, new { CustomerID = customerId }) ?? string.Empty;
            }
        }
        public void SaveFileMetadata(int crmId, string fileName, string filePath)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                INSERT INTO CRM_Files (CRMId, FileName, FilePath, CreatedAt) 
                VALUES (@CRMId, @FileName, @FilePath, GETDATE());";

                connection.Execute(query, new
                {
                    CRMId = crmId,
                    FileName = fileName,
                    FilePath = filePath
                });
            }
        }
        public List<CRMFileViewModel> GetFilesByCRMId(int crmId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT Id, CRMId, FileName, FilePath, CreatedAt
                    FROM CRM_Files
                    WHERE CRMId = @CRMId;";

                return connection.Query<CRMFileViewModel>(query, new { CRMId = crmId }).ToList();
            }
        }
        public void DeleteFile(int Id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM CRM_Files WHERE Id = @Id";
                connection.Execute(query, new { Id = Id });
            }
        }
        public void DeleteCRMAddresses(int crmId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM CRM_Address WHERE CRMId = @CRMId";
                connection.Execute(query, new { CRMId = crmId });
            }
        }

        public void DeleteCRMContacts(int crmId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM CRM_Contacts WHERE CRMId = @CRMId";
                connection.Execute(query, new { CRMId = crmId });
            }
        }

        public void DeleteCRMFiles(int crmId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM CRM_Files WHERE CRMId = @CRMId";
                connection.Execute(query, new { CRMId = crmId });
            }
        }
        public void DeleteCRMRecord(int crmId) {
            using (var connection = new SqlConnection(_connectionString)) {
                // Delete from CRM table
                string deleteCRMQuery = "DELETE FROM CRM WHERE Id = @CRMId";
                connection.Execute(deleteCRMQuery, new { CRMId = crmId });
    
                // Delete related contacts
                DeleteCRMContacts(crmId);
    
                // Delete related addresses
                DeleteCRMAddresses(crmId);
    
                // Delete related files
                DeleteCRMFiles(crmId);
            }
        }
    }
}
