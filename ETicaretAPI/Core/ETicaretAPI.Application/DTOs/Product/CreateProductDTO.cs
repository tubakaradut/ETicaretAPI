﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Product
{
    public class CreateProductDTO
    {
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
    }
}
