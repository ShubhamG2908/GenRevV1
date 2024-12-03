using System.Collections.Generic;

namespace Genrev.Web.App.Data.Models.Forecast
{
    public class ForecastDetailVM
    {
        public bool IsLocked { get; set; }
        public List<ForecastGridItemVM> ForecastListItems { get; set; }

        public IEnumerable<ColumnDefinition> GetColumns()
        {
            return new List<ColumnDefinition> {
                new ColumnDefinition {
                    Name = "Sales",
                    Caption = "Sales",
                    Fields = new FieldDefinition[] {
                        new FieldDefinition {
                            FieldName = "SalesForecast",
                            Caption= "Forecast",
                            Format = "c0"
                        },
                        new FieldDefinition {
                            FieldName =  "SalesTarget",
                            Caption = "Target",
                            Format = "c0"
                        }
                    }
                },
                new  ColumnDefinition {
                    Name = "GPP",
                    Caption = "GPP",
                    Fields = new FieldDefinition[] {
                        new FieldDefinition {
                            FieldName = "GPPForecast",
                            Caption= "Forecast",
                            Format = "{0:n2} %"
                        },
                        new FieldDefinition {
                            FieldName =  "GPPTarget",
                            Caption = "Target",
                            Format = "{0:n2} %"
                        }
                    }
                },
               new ColumnDefinition {
                    Name = "Calls",
                    Caption = "Calls",
                    Fields = new FieldDefinition[] {
                        new FieldDefinition {
                            FieldName = "CallsForecast",
                            Caption= "Forecast",
                        },
                        new FieldDefinition {
                            FieldName =  "CallsTarget",
                            Caption = "Target",
                        }
                    }
               },
                new ColumnDefinition {
                    Name = "Opportunity",
                    Caption = "Opportunity",
                    Fields = new FieldDefinition[] {
                        new FieldDefinition {
                            FieldName = "Potential",
                            Caption= "Potential",
                            Format = "c0"
                        },
                        new FieldDefinition {
                            FieldName = "CurrentOpportunity",
                            Caption= "Current",
                            Format = "c0"
                        },
                        new FieldDefinition {
                            FieldName =  "FutureOpportunity",
                            Caption = "Future",
                            Format = "c0"
                        }
                    }
                }
            };
        }
    }

    public class ColumnDefinition
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public IEnumerable<FieldDefinition> Fields { get; set; }
    }

    public class FieldDefinition
    {
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string Format { get; set; }
    }
}
