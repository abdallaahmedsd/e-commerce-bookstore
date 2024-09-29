$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    const dataTable = $('#tblTable').DataTable({
        ajax: {
            url: '/api/admin/companies'
        },
        columns: [
            { data: 'name', 'width': '20%' },
            { data: 'streetAddress', 'width': '20%' },
            { data: 'city', 'width': '15%' },
            { data: 'state', 'width': '10%' },
            { data: 'phoneNumber', 'width': '15%', className: 'text-start' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/company/details/${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-info-lg"></i>
                            </a>
                            <a href="/admin/company/edit/${data}" class="btn btn-warning mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a href="/admin/company/delete/${data}" class="btn btn-danger mx-2">
                                <i class="bi bi-trash-fill"></i>
                            </a>
                        </div>`;
                },
                'width': '20%',
                orderable: false
            }
        ],
        autoWidth: false // Ensures manual column widths are applied properly
    });
}

