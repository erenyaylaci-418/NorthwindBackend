using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.InMemory;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService;
      

        public ProductManager(IProductDal productDal,ICategoryService categoryService)
        {
            
            _productDal = productDal;
            _categoryService = categoryService;
            
        }
        [SecuredOperation("product.add,admin")]
        [ValidationAspect(typeof(ProductValidator))]
        public IResult Add(Product product)
        {
            IResult result = BusinessRules.Run(CheckIfProductCountOfCategory(product.CategoryId), CheckIfProductNameExists(product.ProductName));
            if (result.Success)
            {
                _productDal.Add(product);
                return new SuccessResult(Messages.ProductAdded);
            }

            return new ErrorResult();
                
          
           
        }
        //[CacheAspect]//key,value
        public IDataResult<List<Product>> GetAll()
        {
            //İş kodları
            //Yetkisi var mı?
           if (DateTime.Now.Hour == 10)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }

            return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductsListed);
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.CategoryId==id));
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.UnitPrice>=min && p.UnitPrice<=max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }

        [ValidationAspect(typeof(ProductValidator))]
        public IResult Update(Product product)
        {
            IResult result = BusinessRules.Run(CheckIfProductCountOfCategory(product.CategoryId), CheckIfProductNameExists(product.ProductName));
            if (result.Success)
            {
                _productDal.Update(product);
                return new SuccessResult(Messages.ProductAdded);
            }

            return new ErrorResult();
        }


        private IResult CheckIfProductCountOfCategory(int categoryId)
        {
            var categoryProductCount = _productDal.GetAll(p => p.CategoryId == categoryId).Count;

            if (categoryProductCount >= 10)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }

            return new SuccessResult();
        }
        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }
        private IResult CheckIfCategoryCount()
        {
          
            var result = _categoryService.GetAll().Data.Count;
            if (result > 15)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }

        public IResult AddTransactionalTest(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
