import { CartItem } from "./cartitem";

export interface Cart{
    id?: number;
    userid: number;
    promoCode: string
    total: number;
    created: Date;
    createdBy: number;
    updated: Date;
    updatedBy: number;

    cartItems: CartItem[]

}