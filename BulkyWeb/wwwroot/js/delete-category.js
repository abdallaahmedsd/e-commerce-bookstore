import '../lib/toastr.js/toastr.min.js';

document.getElementById('js-delete-category').addEventListener('click', (e) => {
    const { categoryId } = e.target.dataset;
    deleteCategory(`/api/admin/categories/${categoryId}`);
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
                success: function (data) {
                    // Store the notification message in sessionStorage
                    sessionStorage.setItem('toastr-success-message', data.message);

                    // Redirect to the categories list page
                    window.location.href = '/admin/category/index';
                }
            });
        }
    });
}