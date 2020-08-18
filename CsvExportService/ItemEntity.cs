using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CsvExportService
{
    public class ItemEntity
    {        
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
