"use client";

import { useState } from "react";
import Link from "next/link";
import { Trash2, Plus, Minus, ShoppingBag } from "lucide-react";

interface CartItem {
    id: number;
    name: string;
    price: number;
    quantity: number;
    image?: string;
}

export default function CartPage() {
    // Mock cart data - in real app this would come from state management or API
    const [cartItems, setCartItems] = useState<CartItem[]>([
        { id: 1, name: "High-Performance Laptop", price: 1299.99, quantity: 1 },
        { id: 2, name: "Wireless Headphones", price: 199.99, quantity: 2 },
    ]);

    const updateQuantity = (id: number, change: number) => {
        setCartItems(
            cartItems.map((item) =>
                item.id === id ? { ...item, quantity: Math.max(1, item.quantity + change) } : item
            )
        );
    };

    const removeItem = (id: number) => {
        setCartItems(cartItems.filter((item) => item.id !== id));
    };

    const subtotal = cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
    const tax = subtotal * 0.1;
    const total = subtotal + tax;

    return (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            <h1 className="text-3xl font-extrabold text-foreground mb-8">Shopping Cart</h1>

            {cartItems.length === 0 ? (
                <div className="text-center py-12">
                    <ShoppingBag className="mx-auto h-16 w-16 text-muted-foreground mb-4" />
                    <p className="text-xl text-muted-foreground mb-4">Your cart is empty</p>
                    <Link
                        href="/products"
                        className="inline-block px-6 py-3 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
                    >
                        Continue Shopping
                    </Link>
                </div>
            ) : (
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                    <div className="lg:col-span-2 space-y-4">
                        {cartItems.map((item) => (
                            <div
                                key={item.id}
                                className="bg-card border border-border rounded-lg p-4 flex items-center gap-4"
                            >
                                <div className="w-24 h-24 bg-muted rounded-md flex items-center justify-center flex-shrink-0">
                                    <span className="text-xs text-muted-foreground">Image</span>
                                </div>
                                <div className="flex-grow">
                                    <h3 className="text-lg font-semibold text-foreground">{item.name}</h3>
                                    <p className="text-primary font-bold mt-1">${item.price.toFixed(2)}</p>
                                </div>
                                <div className="flex items-center gap-2">
                                    <button
                                        onClick={() => updateQuantity(item.id, -1)}
                                        className="p-1 border border-border rounded hover:bg-accent"
                                    >
                                        <Minus className="h-4 w-4" />
                                    </button>
                                    <span className="w-12 text-center font-medium">{item.quantity}</span>
                                    <button
                                        onClick={() => updateQuantity(item.id, 1)}
                                        className="p-1 border border-border rounded hover:bg-accent"
                                    >
                                        <Plus className="h-4 w-4" />
                                    </button>
                                </div>
                                <button
                                    onClick={() => removeItem(item.id)}
                                    className="p-2 text-destructive hover:bg-destructive/10 rounded"
                                >
                                    <Trash2 className="h-5 w-5" />
                                </button>
                            </div>
                        ))}
                    </div>

                    <div className="lg:col-span-1">
                        <div className="bg-card border border-border rounded-lg p-6 sticky top-20">
                            <h2 className="text-xl font-bold text-foreground mb-4">Order Summary</h2>
                            <div className="space-y-3 mb-4">
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">Subtotal</span>
                                    <span className="text-foreground font-medium">${subtotal.toFixed(2)}</span>
                                </div>
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">Tax (10%)</span>
                                    <span className="text-foreground font-medium">${tax.toFixed(2)}</span>
                                </div>
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">Shipping</span>
                                    <span className="text-foreground font-medium">Free</span>
                                </div>
                                <div className="border-t border-border pt-3">
                                    <div className="flex justify-between">
                                        <span className="text-lg font-bold text-foreground">Total</span>
                                        <span className="text-lg font-bold text-primary">${total.toFixed(2)}</span>
                                    </div>
                                </div>
                            </div>
                            <button className="w-full py-3 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors font-medium">
                                Proceed to Checkout
                            </button>
                            <Link
                                href="/products"
                                className="block text-center mt-4 text-sm text-primary hover:text-primary/80"
                            >
                                Continue Shopping
                            </Link>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
