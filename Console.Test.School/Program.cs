using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consoleo.Test.School
{
    
    internal class Program
    {
        static void Main(string[] args)
        {
            clsPerson clsPerson;
            clsPerson =clsPerson.Find(55);
            Console.WriteLine("found it" );
        }
    }
}
