let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblTable').DataTable({
        ajax: {
            url: '/admin/user/getAll'
        },
        columns: [
            { data: 'name', 'width': '20%' },
            { data: 'email', 'width': '15%' },
            { data: 'phone', 'width': '15%' },
            { data: 'company', 'width': '15%' },
            { data: 'role', 'width': '10%' },
            {
                data: {
                    id: 'id',
                    isLocked: 'isLocked'
                },
                render: function (data) {
                    let lockUnlockString = "Lock";
                    let lockUnlockIcon = "lock";
                    let buttonColor = "success";

                    console.log(data);

                    if (data.isLocked) {
                        lockUnlockString = "Unlock";
                        lockUnlockIcon = "unlock";
                        buttonColor = "danger";
                    }

                    return `

                        <div class="btn-group" role="group">
                            <a onclick="lockUnlock(${data.id})" class="btn btn-${buttonColor} mx-2" style="width: 100px">
                                <i class="bi bi-${lockUnlockIcon}-fill"></i> ${lockUnlockString}
                            </a>
                            <a href="/admin/user/roleManagement?userId=${data.id}" class="btn btn-warning">
                                <i class="bi bi-pencil-square"></i> Permissions
                            </a>
                        </div>`;

                },
                'width': '25%',
                orderable: false
            }
        ],
        autoWidth: false // Ensures manual column widths are applied properly
    });
}

function lockUnlock(userId) {
    $.ajax({
        type: 'POST',
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(userId),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                dataTable.ajax.reload();
                toastr.success(data.message);
            }
            else {
                toastr.error(data.message);
            }
        }
    });
}

