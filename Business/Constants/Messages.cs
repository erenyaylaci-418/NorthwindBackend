using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Business.Constants
{
    public static class Messages
    {
        public static string ProductAdded = "Ürün Eklendi";
        public static string ProductNameInvalid = "Ürün ismi geçersiz";
        public  static string MaintenanceTime = "Sistem Bakımda" ;
        public static string ProductsListed="Ürünler Listelendi";
        public  static string ProductCountOfCategoryError;
        public static string ProductNameAlreadyExists;
        public static string AuthorizationDenied ="Yetkiniz yok";
    }
}
