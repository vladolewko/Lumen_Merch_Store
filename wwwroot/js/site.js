// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Cart functionality
document.addEventListener('DOMContentLoaded', function() {
    // Add to cart functionality
    document.querySelectorAll('.add-to-cart-btn').forEach(function(button) {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            const productId = parseInt(this.dataset.productId);
            const sizeId = this.dataset.sizeId ? parseInt(this.dataset.sizeId) : null;
            const quantity = parseInt(this.dataset.quantity) || 1;
            
            addToCart(productId, quantity, sizeId, this);
        });
    });
});

function addToCart(productId, quantity, sizeId, buttonElement) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value 
        || document.querySelector('meta[name="csrf-token"]')?.content;
    
    // Disable button and show loading state
    if (buttonElement) {
        buttonElement.disabled = true;
        buttonElement.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span>';
    }
    
    fetch('/Cart/Add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token || ''
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity,
            sizeId: sizeId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Update cart badge
            updateCartBadge(data.cartCount);
            
            // Show success feedback
            if (buttonElement) {
                buttonElement.innerHTML = '✓ Added!';
                buttonElement.classList.remove('btn-primary');
                buttonElement.classList.add('btn-success');
                
                setTimeout(() => {
                    buttonElement.innerHTML = buttonElement.dataset.originalText || 'Add to Cart';
                    buttonElement.classList.remove('btn-success');
                    buttonElement.classList.add('btn-primary');
                    buttonElement.disabled = false;
                }, 1500);
            }
            
            showToast('Product added to cart!', 'success');
        } else {
            showToast(data.message || 'Error adding to cart', 'error');
            if (buttonElement) {
                buttonElement.disabled = false;
                buttonElement.innerHTML = buttonElement.dataset.originalText || 'Add to Cart';
            }
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showToast('Error adding to cart', 'error');
        if (buttonElement) {
            buttonElement.disabled = false;
            buttonElement.innerHTML = buttonElement.dataset.originalText || 'Add to Cart';
        }
    });
}

function updateCartBadge(count) {
    let badge = document.getElementById('cart-badge');
    const cartLink = document.querySelector('.cart-link');
    
    if (count > 0) {
        if (!badge && cartLink) {
            badge = document.createElement('span');
            badge.id = 'cart-badge';
            badge.className = 'cart-badge';
            cartLink.appendChild(badge);
        }
        if (badge) {
            badge.textContent = count;
            badge.style.display = 'inline';
        }
    } else if (badge) {
        badge.style.display = 'none';
    }
}

function showToast(message, type) {
    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999;';
        document.body.appendChild(toastContainer);
    }
    
    const toast = document.createElement('div');
    toast.className = `alert alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show`;
    toast.style.cssText = 'min-width: 250px; box-shadow: 0 4px 12px rgba(0,0,0,0.3);';
    toast.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    toastContainer.appendChild(toast);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 150);
    }, 3000);
}

// Store original button text on load
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.add-to-cart-btn').forEach(function(button) {
        button.dataset.originalText = button.innerHTML.trim();
    });
});
