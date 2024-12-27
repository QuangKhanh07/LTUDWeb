﻿using SV21T1020037.DomainModels;

namespace SV21T1020037.Web.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}