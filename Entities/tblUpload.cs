using System;

namespace CASIO_HariKrishna.Entities
{
    public class tblUpload
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
