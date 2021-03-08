using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakery.Shared.TransferModel
{
    public class UploadedFile
    {
        public UploadedFile(string name, byte[] vs)
        {
            FileName = name;
            FileContent = vs;
        }

        public UploadedFile()
        {

        }

        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
