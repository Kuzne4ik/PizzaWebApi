import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CartItem } from '../models/cartitem';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  userId: number = 1;
  cartId: number = 1;
  cartItems: CartItem[] = [];
  error: string = '';
  isBusy = false;
  total: number = 0;

  checkoutForm!: FormGroup;

  constructor(private cartService: CartService, private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.reload();  

    this.checkoutForm = this.formBuilder.group({
      lastName: new FormControl('', Validators.required),
      firstName: new FormControl('', Validators.required),
      surName: new FormControl('', ),
      phone: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      address: new FormControl('', )
    });
  }

  get lastName() {return this.checkoutForm.get('lastName')}
  get firstName() {return this.checkoutForm.get('firstName')}
  get surName() {return this.checkoutForm.get('surName')}
  get email() {return this.checkoutForm.get('email')}
  get phone() {return this.checkoutForm.get('phone')}
  get address() {return this.checkoutForm.get('address')}

  reload(){
    this.error = '';
    this.isBusy = true;
    this.cartService.getCart(this.cartId)
    .subscribe({
      next: res => {
        this.cartItems = res.cartItems;
        this.cartService.getTotal(this.cartId).subscribe(
          {
            next: res => this.total = res,
            error:  (err:Error) => this.error = err.message
          }
        )
      }, 
      error:  (err:Error) => this.error = err.message,
      complete: () => this.isBusy = false
    });
  }

  addOneProduct(productId?:number){
    if(!productId)
      return;
    let qty: number = 0;
    this.error = '';
    let cartItem: CartItem | undefined = this.cartItems.find(t => t.productId === productId);
    if(!cartItem)
      return;
    qty = cartItem.quantity + 1
      
    this.cartService.addCartItemToCart(this.cartId, productId)
    .subscribe({
      next: () => {
        (cartItem as CartItem).quantity = qty;
        this.cartService.getTotal(this.cartId).subscribe(
          {
            next: res => this.total = res,
            error:  (err:Error) => this.error = err.message
          }
        )},
      error:  (err:Error) => this.error = err.message
    });
  }

  removeOneProduct(productId?:number){
    if(!productId)
      return;
    let qty: number = 0;
    this.error = '';
    let cartItem: CartItem | undefined = this.cartItems.find(t => t.productId === productId);
    if(!cartItem)
      return;
    qty = cartItem.quantity - 1
         
    this.cartService.updateCartItem(this.cartId, productId, qty)
      .subscribe({
        next: () => {
          (cartItem as CartItem).quantity = qty;
          this.cartService.getTotal(this.cartId).subscribe(
            {
              next: res => this.total = res,
              error:  (err:Error) => this.error = err.message
            }
          )
        },
        error:  (err:Error) => this.error = err.message
      });
  }

  deleteCartItem(productId?:number){
    if(!productId)
      return;
    this.cartService.deleteCartItemFromCart(this.cartId, productId)
    .subscribe({
      next: () => 
      {
        this.cartItems = this.cartItems.filter(t => t.productId !== productId);
        this.cartService.getTotal(this.cartId).subscribe(
          {
            next: res => this.total = res,
            error:  (err:Error) => this.error = err.message
          }
        )
      },
      error:  (err:Error) => this.error = err.message
    });
  }

  onSubmit(){
    if(this.checkoutForm.invalid)
      return;
    const formData = {...this.checkoutForm.value}

  }
}
