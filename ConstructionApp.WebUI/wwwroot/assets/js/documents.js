/**
 * DataTables Basic
 */

'use strict';

// datatable (jquery)
$(function () {
  //  For Datatable
  // --------------------------------------------------------------------
  var dt_documents_table = $('.datatables-documents');

  if (dt_documents_table.length) {
    var dt_documents = dt_documents_table.DataTable({
      
      ajax: assetsPath + 'json/documents.json',
      columns: [
        { data: '' },
        { data: 'id' },
        { data: 'document_name' },
        { data: 'date' },
        { data: '' },
        { data: '' },
      ],
      columnDefs: [
        {
          // For Responsive
          className: 'control',
          searchable: false,
          orderable: false,
          responsivePriority: 2,
          targets: 0,
          render: function () {
            return '';
          }
        },
        {
          // For Checkboxes
          orderable: false,
          searchable: false,
          responsivePriority: 3,
          visible: false,
          checkboxes: true,
          render: function () {
            return '<input type="checkbox" class="dt-checkboxes form-check-input">';
          },
          checkboxes: {
            selectAllRender: '<input type="checkbox" class="form-check-input">'
          }
        },
        {
          // Avatar image/badge, Name, and post
          targets: 2,
          responsivePriority: 4,
          render: function (data, type, full) {
              var $project_img = full['project_img']
              var $name = full['document_name'],
                  $date = full['date'];
               var $project_img_output =
                '<img class="img-project" src="' + assetsPath + 'image/' + $project_img + '" alt="Avatar" class="rounded-circle">';
                  
            return (
              '<div class="d-flex justify-content-left align-items-center">' +
              '<div class="avatar-wrapper">' +
              '<div class="avatar avatar-sm me-3">' +
              $project_img_output+
              '</div>' +
              '</div>' +
              '<div class="d-flex flex-column">' +
              '<h6 class="text-truncate text-document mb-0">' +
              
              $name +
              '</h6>' +
              '</div>' +
              '</div>'
            );
          }
        },
        {
          // Teams
          targets: 4,
          orderable: false,
          searchable: false,
          render: function (data, type, full) {
            var $team = full['access'] || [],
                $team_item = '',
                $team_count = 0;
            for (var i = 0; i < $team.length; i++) {
              $team_item +=
                '<li class="avatar avatar-sm pull-up">' +
                '<img class="rounded-circle" src="' +
                assetsPath +
                'img/avatars/' +
                $team[i] +
                '" alt="Avatar">' +
                '</li>';
              $team_count++;
              if ($team_count > 2) break;
            }
            if ($team_count > 2) {
              var $remainingAvatars = $team.length - 3;
              if ($remainingAvatars > 0) {
                $team_item +=
                  '<li class="avatar avatar-sm">' +
                  '<span class="avatar-initial rounded-circle pull-up text-heading">+' +
                  $remainingAvatars +
                  '</span>' +
                  '</li>';
              }

            }
              // If no team members, display "Only You"
              if ($team_count === 0) {
                $team_item = '<span class="">Only You</span>';
            }
            return (
              '<div class="d-flex align-items-center">' +
              '<ul class="list-unstyled d-flex align-items-center avatar-group mb-0 z-2">' +
              $team_item +
              '</ul>' +
              '<div class="action-button d-flex gap-2 hover-action-button ms-auto">' +  // Removed the button and replaced with icon
                    '<a href="javascript:;" data-bs-toggle="modal" data-bs-target="#invitemember" class="btn btn-outline-primary waves-effect">' +
                        '<i class="fa fa-eye me-1"></i> view ' + 
                    '</a>' + 
                    '<a href="javascript:;" data-bs-toggle="modal" data-bs-target="#invitemember" class="btn btn-outline-primary waves-effect">' +
                        '<i class="fa fa-plus me-1"></i> Share ' + 
                    '</a>' + 
                '</div>' +
              '</div>'
            );
          }
        },
       
        {
          // Actions
          targets: -1,
          searchable: false,
          orderable: false,
          title: 'Action',
          render: function () {
            return (
                
              '<div class="d-inline-block">' +
                // Dropdown button
                '<a href="javascript:;" class="btn btn-icon btn-text-secondary waves-effect waves-light rounded-pill dropdown-toggle hide-arrow" data-bs-toggle="dropdown">' +
                '<i class="ti ti-dots-vertical ti-md"></i>' +
                '</a>' +
                '<div class="dropdown-menu dropdown-menu-end m-0">' +
                // Details button
                '<a href="javascript:;" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#previewModal">Details</a>' +
                 // Details button
                 '<a href="javascript:;" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#invitemember">Share</a>' +
                 // Details button
                 '<a href="assets/image/background-auth.webp" class="dropdown-item" download>Download</a>' +
                 // Rename button
                '<a href="javascript:;" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#renameModal">Rename</a>' +
                // Divider
                '<div class="dropdown-divider"></div>' +
                // Delete button
                '<a href="javascript:;" class="dropdown-item text-danger" data-bs-toggle="modal" data-bs-target="#deleteModal">Delete</a>' +
                '</div>' +
                
                // Modal for Details
                '<div class="modal fade" id="detailsModal" tabindex="-1" aria-labelledby="detailsModalLabel" aria-hidden="true">' +
                '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                    '<div class="modal-header">' +
                        '<h5 class="modal-title" id="detailsModalLabel">Project Details</h5>' +
                        '<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>' +
                    '</div>' +
                    '<div class="modal-body">' +
                        '<p>Details content goes here.</p>' +
                    '</div>' +
                    '<div class="modal-footer">' +
                        '<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>' +
                    '</div>' +
                    '</div>' +
                '</div>' +
                '</div>' +

                // Modal for Rename
                '<div class="modal fade" id="renameModal" tabindex="-1" aria-labelledby="renameModalLabel" aria-hidden="true">' +
                '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                    '<div class="modal-header">' +
                        '<h5 class="modal-title" id="renameModalLabel">Rename Project</h5>' +
                        '<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>' +
                    '</div>' +
                    '<div class="modal-body">' +
                        '<form>' +
                        '<div class="mb-3">' +
                            '<label for="renameProject" class="form-label">New Project Name</label>' +
                            '<input type="text" class="form-control" id="renameProject">' +
                        '</div>' +
                        '</form>' +
                    '</div>' +
                    '<div class="modal-footer">' +
                        '<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>' +
                        '<button type="button" class="btn btn-primary">Save Changes</button>' +
                    '</div>' +
                    '</div>' +
                '</div>' +
                '</div>' +

                // Modal for Delete
                '<div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">' +
                '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                    '<div class="modal-header">' +
                        '<h5 class="modal-title" id="deleteModalLabel">Delete Project</h5>' +
                        '<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>' +
                    '</div>' +
                    '<div class="modal-body">' +
                        '<p>Are you sure you want to delete this project?</p>' +
                    '</div>' +
                    '<div class="modal-footer">' +
                        '<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>' +
                        '<button type="button" class="btn btn-danger">Delete</button>' +
                    '</div>' +
                    '</div>' +
                '</div>' +
                '</div>' +
            '</div>'
            );
          }
        }
      ],
      order: [[1, 'asc']],
      dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
           't' +
           '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
      displayLength: 10,
      lengthMenu: [5, 10, 25, 50, 75, 100],
      language: {
        search: '',
        searchPlaceholder: 'Search Project',
        paginate: {
          next: '<i class="ti ti-chevron-right ti-sm"></i>',
          previous: '<i class="ti ti-chevron-left ti-sm"></i>'
        },
        lengthMenu: "Show _MENU_ entries",
        info: "Showing _START_ to _END_ of _TOTAL_ projects"
      },
      responsive: {
        details: {
          display: $.fn.dataTable.Responsive.display.modal({
            header: function (row) {
              var data = row.data();
              return 'Details of "' + data['project_name'] + '" Project';
            }
          }),
          type: 'column',
          renderer: function (api, rowIdx, columns) {
            var data = $.map(columns, function (col) {
              return col.title
                ? '<tr><td>' + col.title + ':</td><td>' + col.data + '</td></tr>'
                : '';
            }).join(''); 
            return data ? $('<table class="table"/><tbody />').append(data) : false;
          }
        }
      }
    });
  
    $('div.head-label').html('<h5 class="card-title mb-0">Project List</h5>');
}

  
  // Filter form control to default size
  // ? setTimeout used for multilingual table initialization
  setTimeout(() => {
    $('.dataTables_filter .form-control').removeClass('form-control-sm');
    $('.dataTables_length .form-select').removeClass('form-select-sm');
  }, 300);
});
