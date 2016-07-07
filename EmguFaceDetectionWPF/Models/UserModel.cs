using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguFaceDetectionWPF.Models
{
    public class UserModel
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public byte[] Face { get; set; }
    }
}
