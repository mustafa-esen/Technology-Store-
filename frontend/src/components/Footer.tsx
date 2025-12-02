"use client";

export function Footer() {
    return (
        <footer className="bg-background border-t border-border mt-auto">
            <div className="max-w-7xl mx-auto py-12 px-4 sm:px-6 md:flex md:items-center md:justify-between lg:px-8">
                <div className="flex justify-center space-x-6 md:order-2">
                    <a href="#" className="text-muted-foreground hover:text-primary">
                        <span className="sr-only">Facebook</span>
                        {/* Icon placeholder */}
                    </a>
                    <a href="#" className="text-muted-foreground hover:text-primary">
                        <span className="sr-only">Instagram</span>
                        {/* Icon placeholder */}
                    </a>
                    <a href="#" className="text-muted-foreground hover:text-primary">
                        <span className="sr-only">Twitter</span>
                        {/* Icon placeholder */}
                    </a>
                </div>
                <div className="mt-8 md:mt-0 md:order-1">
                    <p className="text-center text-base text-muted-foreground">
                        &copy; {new Date().getFullYear()} Tech Store. Tüm hakları saklıdır.
                    </p>
                </div>
            </div>
        </footer>
    );
}
