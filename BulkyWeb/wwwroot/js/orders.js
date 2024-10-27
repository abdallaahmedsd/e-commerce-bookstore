$(document).ready(function () {
    let statuses = document.querySelectorAll('.js-status-container li');
    let selectedStatus = "all"; // Default selected status

    statuses.forEach(status => {
        status.addEventListener('click', (event) => {
            event.preventDefault(); // Prevent link navigation

            // Remove 'active' class from all and add it to the clicked one
            statuses.forEach(s => s.classList.remove('active'));
            status.classList.add('active');

            // Get the data-status value from the clicked item
            selectedStatus = status.dataset.status;

            // Call loadDataTable with the selected status
            loadDataTable(selectedStatus);
        });
    });

    // Initially load the table with the default status
    loadDataTable(selectedStatus);
});

function loadDataTable(selectedStatus) {
    console.log(selectedStatus);

    // Clear any existing DataTable before loading new data
    if ($.fn.DataTable.isDataTable('#tblTable')) {
        $('#tblTable').DataTable().clear().destroy();
    }

    const dataTable = $('#tblTable').DataTable({
        ajax: {
            url: '/api/admin/orders?status=' + selectedStatus
        },
        columns: [
            { data: 'id', 'width': '5%', className: 'text-start' },
            { data: 'name', 'width': '20%' },
            { data: 'phoneNumber', 'width': '10%', className: 'text-start' },
            { data: 'user.email', 'width': '20%' },
            { data: 'orderStatus', 'width': '15%' },
            { data: 'orderTotal', 'width': '10%', className: 'text-start' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-warning mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>`;
                },
                'width': '15%',
                orderable: false
            }
        ]
    });
}
