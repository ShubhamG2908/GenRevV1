using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.DomainServices.AdHoc.Tests
{
    class Program
    {
        static void Main(string[] args) {


            try {

                //Companies.PersonnelDownstreamHierarchyTest downstreamHierarchyTest = new Companies.PersonnelDownstreamHierarchyTest();
                //downstreamHierarchyTest.TestMe();
                
                    
                Console.WriteLine("Success!");

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }


            Console.Write("Press any key to continue... ");
            Console.ReadKey();
            




        }
    }
}
