import '../lib/toastr.js/toastr.min.js';

document.getElementById('js-delete-book').addEventListener('click', (e) => {
    const { bookId } = e.target.dataset;
    deleteBook(`/api/admin/books/${bookId}`);
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
                    // Store the notification message in sessionStorage
                    sessionStorage.setItem('bookDeleted', data.message);

                    // Redirect to the book list page
                    window.location.href = '/admin/book/index';
                }
            });
        }
    });
}