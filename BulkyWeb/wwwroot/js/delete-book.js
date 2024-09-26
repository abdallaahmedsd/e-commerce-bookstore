import '../lib/toastr.js/toastr.min.js';

document.getElementById('js-delete-book').addEventListener('click', (e) => {
    const { bookId } = e.target.dataset;
    const url = `/api/admin/books/${bookId}`;
    deleteBook(url);
});

function deleteBook(url) {
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
                success: function (data) {
                    if (data.success) {
                        // Store the success notification message in sessionStorage
                        sessionStorage.setItem('toastr-success-message', data.message);

                        // Redirect to the book list page
                        window.location.href = '/admin/book/index';
                    } else {
                        // Handle case where success is false but not a server error
                        toastr.error(data.message || "An unexpected error occurred.");
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