using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFImgApp.ViewModel;

namespace WPFImgApp.Models
{
    class PerPixelOperation :BaseViewModel
    {
        public delegate byte OperationDelegate(byte a, byte b, double opacity = 1);
        public string Name { get; set; }
        public OperationDelegate ByteOperation { get; set; }


        private static List<PerPixelOperation> _operationsList;
        public static List<PerPixelOperation> getOperationsList()
        {
            return _operationsList ??= new List<PerPixelOperation>()
            {
                new PerPixelOperation()
                {
                    Name = "Normal",
                    ByteOperation = (a, b,o) => (byte)(a*o)
                },

                new PerPixelOperation()
                {
                    Name = "Mult",
                    ByteOperation = (a, b,o) => (byte)( a * b / 255.0 )
                },

                new PerPixelOperation()
                {
                    Name = "Sum",
                    ByteOperation = (a, b,o) => (byte)(a+b)
                },
                new PerPixelOperation()
                {
                    Name = "Sub",
                    ByteOperation = (a, b,o) => (byte)(a-b)
                },
                new PerPixelOperation()
                {
                    Name = "Max",
                    ByteOperation = (a, b,o) => (byte)Math.Max(a,b)
                },
                new PerPixelOperation()
                {
                    Name = "Geom",
                    ByteOperation = (a, b,o) => (byte)Math.Sqrt(a*b)
                },
                new PerPixelOperation()
                {
                    Name = "Sr",
                    ByteOperation = (a, b,o) => (byte) ((a+b)/2)
                },

            };


            
        }

        public PerPixelOperation()
        {
            
        }
    }


}
