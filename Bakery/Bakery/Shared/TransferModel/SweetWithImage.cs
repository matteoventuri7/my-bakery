using Bakery.Shared.DataModel;

namespace Bakery.Shared.TransferModel
{
    public class SweetWithImage
    {
        public SweetWithImage(Sweet s)
        {
            Sweet = s;
        }
        public SweetWithImage()
        {

        }

        public Sweet Sweet { get; set; }
        public UploadedFile ImageFile { get; set; }
    }
}
