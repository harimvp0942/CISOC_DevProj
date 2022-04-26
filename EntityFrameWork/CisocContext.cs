using CASIO_HariKrishna.Entities;
using Microsoft.EntityFrameworkCore;

namespace CASIO_HariKrishna.EntityFrameWork
{
    public class CisocContext : DbContext
    {
        public CisocContext(DbContextOptions<CisocContext> options) : base(options)
        {

        }
        public DbSet<tblUpload> tblUploads { get; set; }

    }
}
