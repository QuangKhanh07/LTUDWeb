namespace SV21T1020037.DataLayers
{
    public interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// Truy vấn và lấy toàn bộ dữ liệu của bảng
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
