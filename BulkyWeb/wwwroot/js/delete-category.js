import '../lib/toastr.js/toastr.min.js';

document.getElementById('js-delete-category').addEventListener('click', (e) => {
    const { categoryId } = e.target.dataset;
    const url = `/api/admin/categories/${categoryId}`;
    deleteCategory(url);
});

function deleteCategory(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        // Store the success notification message in sessionStorage
                        sessionStorage.setItem('toastr-success-message', response.message);

                        // Redirect to the categories list page
                        window.location.href = '/admin/category/index';
                    } else {
                        // Handle case where success is false but not a server error
                        toastr.error(response.message || "An unexpected error occurred.");
                    }
                },
                error: function (xhr) {
                    // Handle server-side error (status 500, etc.)
                    const response = xhr.responseJSON;
                    if (response && response.message) {
                        toastr.error(response.message); // Display the error message using toastr
                    } else {
                        toastr.error("An unknown error occurred."); // Fallback message
                    }
                }
            });
        }
    });
}