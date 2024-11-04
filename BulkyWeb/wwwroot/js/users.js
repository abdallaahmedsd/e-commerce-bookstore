$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    const dataTable = $('#tblTable').DataTable({
        ajax: {
            url: '/admin/user/getAll'
        },
        columns: [
            { data: 'name', 'width': '20%' },
            { data: 'email', 'width': '20%' },
            { data: 'phone', 'width': '20%' },
            { data: 'company', 'width': '15%' },
            { data: 'role', 'width': '15%', className: 'text-start' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/company/edit/${data}" class="btn btn-warning mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>`;
                },
                'width': '10%',
                orderable: false
            }
        ],
        autoWidth: false // Ensures manual column widths are applied properly
    });
}

