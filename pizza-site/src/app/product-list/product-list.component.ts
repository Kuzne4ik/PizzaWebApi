import { Component, OnInit } from '@angular/core';
import { Category } from '../models/category';
import { Product } from '../models/product';
import { SearchResult } from '../models/search_result';
import { CategoryService } from '../services/category.service';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  cacheProducts: Product[] = [];
  products: Product[] = [];
  categories!: Category[];
  product!: Product;
  public selectedCategoryId!: number;
  error: string = '';
  isBusy = false;

  constructor(private productService: ProductService, private categoryService:CategoryService) { }

  ngOnInit(): void {
    this.reload();
    
  }

  reloadCategories(){
    this.error = '';
    this.isBusy = true;
    this.categoryService.searchCategories()
    .subscribe({
      next: res => {
          this.categories = res.results
          if(this.categories.length > 0)
          {
            this.selectedCategoryId = Number(this.categories[0].id);
            this.onCategoryChanged(this.selectedCategoryId);
          }
        }
      , 
      error:  (err:Error) => this.error = err.message,
      complete: () => this.isBusy = false
    });
  }


  reload(){
    this.error = '';
    this.isBusy = true;
    this.productService.searchProducts()
    .subscribe({
      next: res => {
        this.cacheProducts = res.results;
        this.reloadCategories();
      }, 
      error:  (err:Error) => this.error = err.message,
      complete: () => this.isBusy = false
    });
  }

  addProduct(){
    this.error = '';

    this.productService.addProduct('zzz', 'zzz', '111', 10)
    .subscribe({
      next: product => this.products?.push(product), 
      error:  (err:Error) => this.error = err.message
    });
  }

  deleteProduct(id?:number)
  {

  }

  onCategoryChanged(selectedCategoryId:number){
    this.products = this.cacheProducts.filter(t => t.categoryId == selectedCategoryId);
    console.log(this.products)
  }
}
