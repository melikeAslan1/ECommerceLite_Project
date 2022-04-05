using ECommerceLiteBLL.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceLiteEntity.Models;

namespace ECommerceLiteBLL.Repository
{
    public class CategoryRepo : RepositoryBase<Category, int> { }
    public class ProductRepo : RepositoryBase<Product, int> { }
    public class OrderRepo : RepositoryBase<Order, int> { }
    public class OrderDetailRepo : RepositoryBase<OrderDetail, int> { }
    public class CustomerRepo : RepositoryBase<Customer, int> { }
    public class AdminRepo : RepositoryBase<Admin, int> { }
    public class PassiveUserRepo : RepositoryBase<PassiveUser, int> { }
    public class ProductPictureRepo : RepositoryBase<ProductPicture, int> { }
}
