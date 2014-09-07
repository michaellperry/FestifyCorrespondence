using Festify.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.Correspondence.FileStream;

namespace Festify.DallasTechFest._2014
{
    class Program
    {
        static void Main(string[] args)
        {
            Device device = new Device(
                new FileStreamStorageStrategy(
                    Path.Combine(
                        Environment.CurrentDirectory,
                        "Correspondence")));
        }
    }
}
