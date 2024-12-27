namespace SV21T1020037.BusinessLayers;

using SV21T1020037.BusinessLayer;
using SV21T1020037.DataLayers;
using SV21T1020037.DataLayers.SQLServer;
using SV21T1020037.DomainModels;

public static class ProductDataService
{
    private static readonly IProductDAL productDB;

    static ProductDataService()
    {
        productDB = new ProductDAL(Configuration.ConnectionString);
    }

    public static List<Product> ListProducts(string searchValue = "")
    {
        return productDB.List(1, 0, searchValue);
    }

    public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
    {
        rowCount = productDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
        return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice);
    }

    public static Product? GetProduct(int productID)
    {
        return productDB.Get(productID);
    }

    public static int AddProduct(Product data)
    {
        return productDB.Add(data);
    }

    public static bool UpdateProduct(Product data)
    {
        return productDB.Update(data);
    }

    public static bool DeleteProduct(int productID)
    {
        return productDB.Delete(productID);
    }

    public static bool InUsedProduct(int productID)
    {
        return productDB.InUsed(productID);
    }

    public static int GetPhotoID(int productID, string description)
    {
        return productDB.GetPhotoID(productID, description);
    }
    public static int GetAttributeID(int productID, string attributeName)
    {
        return productDB.GetAttributeID(productID, attributeName);
    }

    public static List<ProductPhoto> ListPhotos(int productID)
    {
        return productDB.ListPhotos(productID).ToList();
    }

    public static ProductPhoto? GetPhoto(int productID, int photoID)
    {
        return productDB.GetPhoto(productID, photoID);
    }

    public static long AddPhoto(ProductPhoto data)
    {
        return productDB.AddPhoto(data);
    }

    public static bool UpdatePhoto(ProductPhoto data)
    {
        return productDB.UpdatePhoto(data);
    }

    public static bool DeletePhoto(int photoID)
    {
        return productDB.DeletePhoto(photoID);
    }

    public static List<ProductAttribute> ListAttributes(int productID)
    {
        return productDB.ListAttributes(productID).ToList();
    }

    public static ProductAttribute? ListAttribute(int productID, int attributeID)
    {
        return productDB.GetAttribute(productID, attributeID);
    }

    public static long AddAttribute(ProductAttribute data)
    {
        return productDB.AddAttribute(data);
    }

    public static bool UpdateAttribute(ProductAttribute data)
    {
        return productDB.UpdateAttribute(data);
    }

    public static bool DeleteAttribute(long attributeID)
    {
        return productDB.DeleteAttribute(attributeID);
    }
}
