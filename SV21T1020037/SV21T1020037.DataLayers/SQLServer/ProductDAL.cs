using SV21T1020037.DomainModels;
using Dapper;

namespace SV21T1020037.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                            else
                                begin
                                    insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling) 
                                    values (@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                                    select scope_identity()
                                end;";

                var parameters = new
                {
                    data.ProductName,
                    data.ProductDescription,
                    data.SupplierID,
                    data.CategoryID,
                    data.Unit,
                    data.Price,
                    data.Photo,
                    data.IsSelling
                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder) 
                        values (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                        select scope_identity();";

                var parameters = new
                {
                    data.ProductID,
                    data.AttributeName,
                    data.AttributeValue,
                    data.DisplayOrder
                };

                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            long id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden) 
                        values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                        select scope_identity();";

                var parameters = new
                {
                    data.ProductID,
                    data.Photo,
                    data.Description,
                    data.DisplayOrder,
                    data.IsHidden
                };

                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) 
                        from Products
                        where 
                            (ProductName like @searchValue or @searchValue = '') and
                            (CategoryID = @categoryID or @categoryID = 0) and
                            (SupplierID = @supplierID or @supplierID = 0) and
                            (Price >= @minPrice) and
                            (Price <= @maxPrice or @maxPrice = 0)";

                var parameters = new { searchValue, categoryID, supplierID, minPrice, maxPrice };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = "delete from ProductPhotos where ProductID = @productID;delete from ProductAttributes where ProductID = @productID; delete from Products where ProductID = @productID";
                var parameters = new { productID };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = "delete from ProductAttributes where AttributeID = @attributeID";
                var parameters = new { attributeID };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeletePhoto(int photoID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = "delete from ProductPhotos where PhotoID = @photoID";
                var parameters = new { photoID };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;

            using (var connection = OpenConnection())
            {
                var sql = "select * from Products where ProductID = @productID";
                var parameters = new { productID };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return data;
        }

        public ProductAttribute? GetAttribute(int productID, int attributeID)
        {
            ProductAttribute? attribute = null;

            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductAttributes where ProductID = @productID and AttributeID = @attributeID";
                var parameters = new { productID, attributeID };
                attribute = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return attribute;
        }

        public ProductPhoto? GetPhoto(int productID, int photoID)
        {
            ProductPhoto? photo = null;

            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductPhotos where ProductID = @productID and PhotoID = @photoID";
                var parameters = new { 
                    productID,
                    photoID
                };
                photo = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return photo;
        }

     

        public bool InUsed(int productID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @productID)
                            select 1
                        else
                            select 0;";

                var parameters = new { productID };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                        from (
                            select *, row_number() over(order by ProductName) as RowNumber
                            from Products
                            where 
                                (ProductName like @searchValue or @searchValue = '') and
                                (CategoryID = @categoryID or @categoryID = 0) and
                                (SupplierID = @supplierID or @supplierID = 0) and
                                (Price >= @minPrice) and
                                (Price <= @maxPrice or @maxPrice = 0)
                        ) as t
                        where (@pageSize = 0) 
                        or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";

                var parameters = new
                {
                    searchValue,
                    categoryID,
                    supplierID,
                    minPrice,
                    maxPrice,
                    page,
                    pageSize
                };

                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            IList<ProductAttribute> attributes;

            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductAttributes where ProductID = @productID";
                var parameters = new { productID };
                attributes = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return attributes;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            IList<ProductPhoto> photos;

            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductPhotos where ProductID = @productID";
                var parameters = new { productID };
                photos = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return photos;
        }
        public bool Update(Product data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                                begin
                                    update Products set 
                                    ProductName = @ProductName, 
                                    ProductDescription = @ProductDescription, 
                                    SupplierID = @SupplierID, 
                                    CategoryID = @CategoryID, 
                                    Unit = @Unit, 
                                    Price = @Price, 
                                    Photo = @Photo, 
                                    IsSelling = @IsSelling
                                    where ProductID = @ProductID
                                end";

                var parameters = new
                {
                    data.ProductID,
                    data.ProductName,
                    data.ProductDescription,
                    data.SupplierID,
                    data.CategoryID,
                    data.Unit,
                    data.Price,
                    data.Photo,
                    data.IsSelling
                };

                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes set 
                        AttributeName = @AttributeName, 
                        AttributeValue = @AttributeValue, 
                        DisplayOrder = @DisplayOrder
                        where AttributeID = @AttributeID";

                var parameters = new
                {
                    data.AttributeID,
                    data.AttributeName,
                    data.AttributeValue,
                    data.DisplayOrder
                };

                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos set 
                        Photo = @Photo, 
                        Description = @Description, 
                        DisplayOrder = @DisplayOrder, 
                        IsHidden = @IsHidden
                        where PhotoID = @PhotoID";

                var parameters = new
                {
                    data.PhotoID,
                    data.Photo,
                    data.Description,
                    data.DisplayOrder,
                    data.IsHidden
                };

                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public int GetPhotoID(int productID, string description)
        {
            int photoID = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"select PhotoID 
                    from ProductPhotos 
                    where ProductID = @productID and Description = @description";

                var parameters = new { productID, description };
                photoID = connection.QueryFirstOrDefault<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return photoID;
        }

        public int GetAttributeID(int productID, string attributeName)
        {
            int attributeID = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"select AttributeID 
                    from ProductAttributes 
                    where ProductID = @productID and AttributeName = @attributeName";

                var parameters = new { productID, attributeName };
                attributeID = connection.QueryFirstOrDefault<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return attributeID;
        }



    }
}
