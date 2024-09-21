
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    const dataTable = $('#tblTable').DataTable({
        ajax: {
            url: '/api/admin/books'
        },
        columns: [
            { data: 'title', 'width': '20%' },
            { data: 'author', 'width': '20%' },
            { data: 'isbn', 'width': '20%' },
            { data: 'price', 'width': '10%', className: 'text-start' },
            { data: 'category', 'width': '10%' },
            {
                data: 'id',
                render: function (data) {
                    return `
                                <div class="btn-group" role="group">
                                    <a href="/admin/book/details/${data}" class="btn btn-primary mx-2">
                                        <i class="bi bi-info-lg"></i>
                                    </a>
                                    <a href="/admin/book/edit/${data}" class="btn btn-warning mx-2">
                                        <i class="bi bi-pencil-square"></i>
                                    </a>
                                    <a href="/admin/book/delete/${data}" class="btn btn-danger mx-2">
                                        <i class="bi bi-trash-fill"></i>
                                    </a>
                                </div>`;
                },
                'width': '20%',
                orderable: false
            }
        ]
    });
}
