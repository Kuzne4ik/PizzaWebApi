import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../models/product';
import { CartService } from '../services/cart.service';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {

  product?: Product;
  error: string = '';
  cartId:number = 1;

  constructor(private route: ActivatedRoute, private productService: ProductService, private cartService: CartService) { }

  ngOnInit(): void {
    const routeParams = this.route.snapshot.paramMap;
    const productId = Number(routeParams.get('productId'));

    this.productService.getProduct(productId)
      .subscribe({
      next: res => this.product = res, 
      error:  (err:Error) => this.error = err.message
    });
  }

  addToCart(product: Product) {
    this.error = '';
    this.cartService.addCartItemToCart(this.cartId, Number(product.id)).subscribe({
      next: ()=>window.alert('Your product has been added to the cart!'),
      error:  (err:Error) => this.error = err.message
    });
    
  }

}
