using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandResponse
    {
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public string ProductId { get; set; }
        public float Price { get; set; }
    }
}
