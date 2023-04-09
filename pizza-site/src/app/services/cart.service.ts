import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams} from '@angular/common/http';
import { CartItem } from '../models/cartitem';
import { catchError, Observable, throwError } from 'rxjs';
import { Product } from '../models/product';
import { SearchResult } from '../models/search_result';
import { Cart } from '../models/cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  //readonly APIUrl = "http://localhost:8009/api";
  readonly APIUrl = "http://localhost:5118/api";
  

  constructor(private http: HttpClient) { }


  addCartItemToCart(cartId: number, productId: number): any {
    
    return this.http.post<CartItem>(this.APIUrl + `/Carts/${cartId}/items?productId=${productId}`, null)
    .pipe(
      catchError(err => this.handleError(err, "Failed to create item"))
    );
  }

  deleteCartItemFromCart(cartId: number, productId:number){
    return this.http.delete<void>(this.APIUrl + `/Carts?cartId=${cartId}&productId=${productId}`)
    .pipe(
      catchError(err => this.handleError(err, "Failed to update Cart"))
    );
  }

  updateCartItem(cartId: number, productId: number, qty: number){
    return this.http.put<CartItem>(this.APIUrl + `/Carts/${cartId}/items?productId=${productId}&qty=${qty}`, null)
    .pipe(
      catchError(err => this.handleError(err, "Failed to update Cart"))
    );
  }

  getCart(cartId:number): Observable<Cart>{
    return this.http.get<Cart>(this.APIUrl + `/carts/${cartId}`)
    .pipe(
      catchError(err => this.handleError(err, "Failed to load cart"))
    );
  }

  clearCart(cartId:number) {
    return this.http.delete<void>(this.APIUrl + `/Carts/${cartId}/items`)
    .pipe(
      catchError(err => this.handleError(err, "Failed to update Cart"))
    );
  }

  getTotal(cartId:number): Observable<number>{
    return this.http.get<number>(this.APIUrl + `/carts/${cartId}/total`)
    .pipe(
      catchError(err => this.handleError(err, "Failed to load cart"))
    );
  }


  private handleError(error: HttpErrorResponse, shortErrorMsg: string) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, body was: `, error.message);
    }
    // Return an observable with a user-facing error message.
    return throwError(() => new Error(shortErrorMsg));
  }
}