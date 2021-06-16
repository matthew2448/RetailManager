using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEnpoint;
        public SalesViewModel(IProductEndpoint productEnpoint)
        {
            _productEnpoint = productEnpoint;
            
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEnpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set { 
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelecetedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value;
                NotifyOfPropertyChange(() => SelecetedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set { _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }


        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal
        {
            get {
                decimal subTotal = 0;

                foreach(var item in Cart)
                {
                    subTotal += (item.Product.RetailPrice * item.QuantityInCart);
                }
                return subTotal.ToString("C");
                //return "$0.00"; 
            
            }
        }
        public string Tax
        {
            get { return "$0.00"; }
        }
        public string Total
        {
            get { return "$0.00"; }
        }


        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //If something is selected
                //If Item has stock
                if (ItemQuantity > 0 && SelecetedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }
        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //If something is selected

                return output;
            }
        }
        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //If something is selected

                return output;
            }
        }
        public void RemoveFromCart()
        {

        }

        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelecetedProduct);
            if(existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                ///Cart.Remove(existingItem);
                ///Cart.Add(existingItem);
                
            }
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelecetedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            
            SelecetedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => existingItem.DisplayText);

        }

        public void CheckOut()
        {

        }
    }
}
