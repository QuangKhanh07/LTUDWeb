﻿using SV21T1020037.DomainModels;

namespace SV21T1020037.DataLayers
{
    public interface IProductDAL
    {
        List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

        Product? Get(int productID);

        int Add(Product data);

        bool Update(Product data);

        bool Delete(int productID);

        bool InUsed(int productID);

        IList<ProductPhoto> ListPhotos(int productID);

        ProductPhoto? GetPhoto(int productID, int photoID);

        long AddPhoto(ProductPhoto data);

        bool UpdatePhoto(ProductPhoto data);
        bool DeletePhoto(int photoID);
        IList<ProductAttribute> ListAttributes(int productID);  
        ProductAttribute? GetAttribute(int productID,int attributeID);
        long AddAttribute(ProductAttribute data);
        bool UpdateAttribute(ProductAttribute data);
        bool DeleteAttribute(long attributeID);
        int GetPhotoID(int productID, string desctiption);
        int GetAttributeID(int productID, string attributeName);
    }
}
