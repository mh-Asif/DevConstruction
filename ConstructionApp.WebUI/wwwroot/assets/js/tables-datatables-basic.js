/**
 * DataTables Basic
 */

'use strict';

let fv, offCanvasEl;
document.addEventListener('DOMContentLoaded', function (e) {
  (function () {
    const formAddNewRecord = document.getElementById('form-add-new-record');

    setTimeout(() => {
      const newRecord = document.querySelector('.create-new'),
        offCanvasElement = document.querySelector('#add-new-record');

      // To open offCanvas, to add new record
      if (newRecord) {
        newRecord.addEventListener('click', function () {
          offCanvasEl = new bootstrap.Offcanvas(offCanvasElement);
          // Empty fields on offCanvas open
          (offCanvasElement.querySelector('.dt-full-name').value = ''),
            (offCanvasElement.querySelector('.dt-post').value = ''),
            (offCanvasElement.querySelector('.dt-email').value = ''),
            (offCanvasElement.querySelector('.dt-date').value = ''),
            (offCanvasElement.querySelector('.dt-salary').value = '');
          // Open offCanvas with form
          offCanvasEl.show();
        });
      }
    }, 200);

    // Form validation for Add new record
    fv = FormValidation.formValidation(formAddNewRecord, {
      fields: {
        basicFullname: {
          validators: {
            notEmpty: {
              message: 'The name is required'
            }
          }
        },
        basicPost: {
          validators: {
            notEmpty: {
              message: 'Post field is required'
            }
          }
        },
        basicEmail: {
          validators: {
            notEmpty: {
              message: 'The Email is required'
            },
            emailAddress: {
              message: 'The value is not a valid email address'
            }
          }
        },
        basicDate: {
          validators: {
            notEmpty: {
              message: 'Joining Date is required'
            },
            date: {
              format: 'MM/DD/YYYY',
              message: 'The value is not a valid date'
            }
          }
        },
        basicSalary: {
          validators: {
            notEmpty: {
              message: 'Basic Salary is required'
            }
          }
        }
      },
      plugins: {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
          // Use this for enabling/changing valid/invalid class
          // eleInvalidClass: '',
          eleValidClass: '',
          rowSelector: '.col-sm-12'
        }),
        submitButton: new FormValidation.plugins.SubmitButton(),
        // defaultSubmit: new FormValidation.plugins.DefaultSubmit(),
        autoFocus: new FormValidation.plugins.AutoFocus()
      },
      init: instance => {
        instance.on('plugins.message.placed', function (e) {
          if (e.element.parentElement.classList.contains('input-group')) {
            e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
          }
        });
      }
    });

    // FlatPickr Initialization & Validation
    const flatpickrDate = document.querySelector('[name="basicDate"]');

    if (flatpickrDate) {
      flatpickrDate.flatpickr({
        enableTime: false,
        // See https://flatpickr.js.org/formatting/
        dateFormat: 'm/d/Y',
        // After selecting a date, we need to revalidate the field
        onChange: function () {
          fv.revalidateField('basicDate');
        }
      });
    }
  })();
});

// datatable (jquery)
$(function () {
  var dt_basic_table = $('.datatables-basic'),
    dt_complex_header_table = $('.dt-complex-header'),
    dt_row_grouping_table = $('.dt-row-grouping'),
    dt_multilingual_table = $('.dt-multilingual'),
    dt_basic;

  // DataTable with buttons
  // --------------------------------------------------------------------
  
  if (dt_basic_table.length) {
    dt_basic = dt_basic_table.DataTable({
      // Use static data instead of ajax
      ajax: assetsPath + 'json/table-datatable.json', // Your JSON data source
  
      columns: [
        { data: null },                    // For the first empty column
        { data: null },                    // For the second empty column
        { data: 'id' },                    // 'id'
        { data: 'project_name' },          // 'Project Name'
        { data: null },                    // For the fifth empty column (set null instead of '')
        { data: 'owner' },                 // 'Owner'
        { data: 'status' },                // 'Status'
        { data: 'tasks' },                 // 'Tasks'
        { data: 'phases' },                // 'Phases'
        { data: 'issues' },                // 'Issues'
        { data: 'start_date' },            // 'Start Date'
        { data: 'end_date' },              // 'End Date'
        { data: 'address' },               // 'Address'
        { data: 'city' },                  // 'City'
        { data: 'state' },                 // 'State'
        { data: 'stage' },                 // 'Stage'
        { data: 'zip' },                   // 'Zip'
        { data: 'phone' },                 // 'Phone'
        { data: 'tags' },                  // 'Tags'
      ],
  
      columnDefs: [
        // Responsive column: control
        {
          className: 'control',
          orderable: false,
          searchable: false,
          responsivePriority: 2,
          targets: 0,
          render: () => ''
        },
        // Checkbox column
        {
          targets: 1,
          orderable: false,
          searchable: false,
          responsivePriority: 3,
          checkboxes: {
            selectAllRender: '<input type="checkbox" class="form-check-input">'
          },
          render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
        },
        // Hidden column for 'id'
        {
          targets: 2,
          searchable: false,
          visible: false
        },
        // Render project name with custom structure
        {
          targets: 3,
          responsivePriority: 4,
          render: (data, type, full) => {
            return `
              <div class="container d-flex justify-content-between align-items-center">
                <div class="d-flex justify-content-start align-items-center user-name">
                  <div class="d-flex flex-column">
                    <span class="emp_name text-truncate">${full['project_name']}</span>
                  </div>
                </div>
              </div>
            `;
          }
        },
        {
          targets: 4,
          visible: false,
        },
        // Render status with a badge
        {
          targets: -2,
          render: (data, type, full) => {
            const $status = full['status'];
            return `<span class="badge bg-label-primary">${$status}</span>`;
          }
        },
        // Action buttons column
        {
          targets: -1,
          title: 'Actions',
          visible: false,
          orderable: false,
          searchable: false,
          render: (data, type, full) => {
            return `
              <div class="d-inline-block">
                <a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                  <i class="ti ti-dots-vertical ti-md"></i>
                </a>
                <ul class="dropdown-menu dropdown-menu-end m-0">
                  <li><a href="javascript:;" class="dropdown-item">Details</a></li>
                  <li><a href="javascript:;" class="dropdown-item">Archive</a></li>
                  <div class="dropdown-divider"></div>
                  <li><a href="javascript:;" class="dropdown-item text-danger delete-record">Delete</a></li>
                </ul>
              </div>
              <a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon item-edit">
                <i class="ti ti-pencil ti-md"></i>
              </a>
            `;
          }
        }
      ],
  
      order: [[2, 'desc']], // Default sorting
      dom: '<"card-header flex-column flex-md-row"<"head-label text-center"><"dt-action-buttons text-end pt-6 pt-md-0"B>><"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end mt-n6 mt-md-0"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      displayLength: 7,
      lengthMenu: [7, 10, 25, 50, 75, 100],
  
      language: {
        paginate: {
          next: '<i class="ti ti-chevron-right ti-sm"></i>',
          previous: '<i class="ti ti-chevron-left ti-sm"></i>'
        }
      },
  
      buttons: [
        // Define any export buttons if needed
      ]
    });
  }
  
  
  

  // Add New record
  // ? Remove/Update this code as per your requirements
  var count = 101;
  // On form submit, if form is valid
  fv.on('core.form.valid', function () {
    var $new_name = $('.add-new-record .dt-full-name').val(),
      $new_post = $('.add-new-record .dt-post').val(),
      $new_email = $('.add-new-record .dt-email').val(),
      $new_date = $('.add-new-record .dt-date').val(),
      $new_salary = $('.add-new-record .dt-salary').val();

    if ($new_name != '') {
      dt_basic.row
        .add({
          id: count,
          full_name: $new_name,
          post: $new_post,
          email: $new_email,
          start_date: $new_date,
          salary: '$' + $new_salary,
          status: 5
        })
        .draw();
      count++;

      // Hide offcanvas using javascript method
      offCanvasEl.hide();
    }
  });

  // Delete Record
  $('.datatables-basic tbody').on('click', '.delete-record', function () {
    dt_basic.row($(this).parents('tr')).remove().draw();
  });

  // Complex Header DataTable
  // --------------------------------------------------------------------

  if (dt_complex_header_table.length) {

    var dt_complex = dt_complex_header_table.DataTable({
      ajax: assetsPath + 'json/table-datatable.json',
      columns: [
        { data: 'full_name' },
        { data: 'email' },
        { data: 'city' },
        { data: 'post' },
        { data: 'salary' },
        { data: 'status' },
        { data: '' }
      ],
      columnDefs: [
        {
          // Label
          targets: -2,
          render: function (data, type, full, meta) {
            var $status_number = full['status'];
            var $status = {
              1: { title: 'Current', class: 'bg-label-primary' },
              2: { title: 'Professional', class: ' bg-label-success' },
              3: { title: 'Rejected', class: ' bg-label-danger' },
              4: { title: 'Resigned', class: ' bg-label-warning' },
              5: { title: 'Applied', class: ' bg-label-info' }
            };
            if (typeof $status[$status_number] === 'undefined') {
              return data;
            }
            return (
              '<span class="badge ' + $status[$status_number].class + '">' + $status[$status_number].title + '</span>'
            );
          }
        },
        {
          // Actions
          targets: -1,
          title: 'Actions',
          orderable: false,
          render: function (data, type, full, meta) {
            return (
              '<div class="d-inline-block">' +
              '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="ti ti-dots-vertical ti-md"></i></a>' +
              '<div class="dropdown-menu dropdown-menu-end m-0">' +
              '<a href="javascript:;" class="dropdown-item">Details</a>' +
              '<a href="javascript:;" class="dropdown-item">Archive</a>' +
              '<div class="dropdown-divider"></div>' +
              '<a href="javascript:;" class="dropdown-item text-danger delete-record">Delete</a>' +
              '</div>' +
              '</div>' +
              '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon item-edit"><i class="ti ti-pencil ti-md"></i></a>'
            );
          }
        }
      ],
      dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end mt-n6 mt-md-0"f>><"table-responsive"t><"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      displayLength: 7,
      lengthMenu: [7, 10, 25, 50, 75, 100],
      language: {
        paginate: {
          next: '<i class="ti ti-chevron-right ti-sm"></i>',
          previous: '<i class="ti ti-chevron-left ti-sm"></i>'
        }
      }
    });
  }

  // Row Grouping
  // --------------------------------------------------------------------

  var groupColumn = 2;
  if (dt_row_grouping_table.length) {
    var groupingTable = dt_row_grouping_table.DataTable({
      ajax: assetsPath + 'json/table-datatable.json',
      scrollX: true, // Enable horizontal scrolling
      fixedColumns: {
        leftColumns: 2 // Freeze the first two columns
      },
      columns: [
        { data: '' },
        { data: 'full_name' },
        { data: 'post' },
        { data: 'email' },
        { data: 'city' },
        { data: 'start_date' },
        { data: 'salary' },
        { data: 'status' },
        { data: '' }
      ],
      columnDefs: [
        {
          // For Responsive
          className: 'control',
          orderable: false,
          targets: 0,
          searchable: false,
          render: function (data, type, full, meta) {
            return '';
          }
        },
        { visible: false, targets: groupColumn },
        {
          // Label
          targets: -2,
          render: function (data, type, full, meta) {
            var $status_number = full['status'];
            var $status = {
              1: { title: 'Current', class: 'bg-label-primary' },
              2: { title: 'Professional', class: ' bg-label-success' },
              3: { title: 'Rejected', class: ' bg-label-danger' },
              4: { title: 'Resigned', class: ' bg-label-warning' },
              5: { title: 'Applied', class: ' bg-label-info' }
            };
            if (typeof $status[$status_number] === 'undefined') {
              return data;
            }
            return (
              '<span class="badge ' + $status[$status_number].class + '">' + $status[$status_number].title + '</span>'
            );
          }
        },
        {
          // Actions
          targets: -1,
          title: 'Actions',
          orderable: false,
          searchable: false,
          render: function (data, type, full, meta) {
            return (
              '<div class="d-inline-block">' +
              '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="ti ti-dots-vertical ti-md"></i></a>' +
              '<div class="dropdown-menu dropdown-menu-end m-0">' +
              '<a href="javascript:;" class="dropdown-item">Details</a>' +
              '<a href="javascript:;" class="dropdown-item">Archive</a>' +
              '<div class="dropdown-divider"></div>' +
              '<a href="javascript:;" class="dropdown-item text-danger delete-record">Delete</a>' +
              '</div>' +
              '</div>' +
              '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon item-edit"><i class="ti ti-pencil ti-md"></i></a>'
            );
          }
        }
      ],
      order: [[groupColumn, 'asc']],
      dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end mt-n6 mt-md-0"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      displayLength: 7,
      lengthMenu: [7, 10, 25, 50, 75, 100],
      language: {
        paginate: {
          next: '<i class="ti ti-chevron-right ti-sm"></i>',
          previous: '<i class="ti ti-chevron-left ti-sm"></i>'
        }
      },
      drawCallback: function (settings) {
        var api = this.api();
        var rows = api.rows({ page: 'current' }).nodes();
        var last = null;

        api
          .column(groupColumn, { page: 'current' })
          .data()
          .each(function (group, i) {
            if (last !== group) {
              $(rows)
                .eq(i)
                .before('<tr class="group"><td colspan="8">' + group + '</td></tr>');

              last = group;
            }
          });
      },
      responsive: {
        details: {
          display: $.fn.dataTable.Responsive.display.modal({
            header: function (row) {
              var data = row.data();
              return 'Details of ' + data['full_name'];
            }
          }),
          type: 'column',
          renderer: function (api, rowIdx, columns) {
            var data = $.map(columns, function (col, i) {
              return col.title !== '' // ? Do not show row in modal popup if title is blank (for check box)
                ? '<tr data-dt-row="' +
                    col.rowIndex +
                    '" data-dt-column="' +
                    col.columnIndex +
                    '">' +
                    '<td>' +
                    col.title +
                    ':' +
                    '</td> ' +
                    '<td>' +
                    col.data +
                    '</td>' +
                    '</tr>'
                : '';
            }).join('');

            return data ? $('<table class="table"/><tbody />').append(data) : false;
          }
        }
      }
    });

    // Order by the grouping
    $('.dt-row-grouping tbody').on('click', 'tr.group', function () {
      var currentOrder = groupingTable.order()[0];
      if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
        groupingTable.order([groupColumn, 'desc']).draw();
      } else {
        groupingTable.order([groupColumn, 'asc']).draw();
      }
    });
  }

  // Multilingual DataTable
  // --------------------------------------------------------------------

  var lang = 'German';
  if (dt_multilingual_table.length) {
    var table_language = dt_multilingual_table.DataTable({
      ajax: assetsPath + 'json/table-datatable.json',
      columns: [
        { data: '' },
        { data: 'full_name' },
        { data: 'post' },
        { data: 'email' },
        { data: 'start_date' },
        { data: 'salary' },
        { data: 'status' },
        { data: '' }
      ],
      columnDefs: [
        {
          // For Responsive
          className: 'control',
          orderable: false,
          targets: 0,
          searchable: false,
          render: function (data, type, full, meta) {
            return '';
          }
        },
        {
          // Label
          targets: -2,
          render: function (data, type, full, meta) {
            var $status_number = full['status'];
            var $status = {
              1: { title: 'Current', class: 'bg-label-primary' },
              2: { title: 'Professional', class: ' bg-label-success' },
              3: { title: 'Rejected', class: ' bg-label-danger' },
              4: { title: 'Resigned', class: ' bg-label-warning' },
              5: { title: 'Applied', class: ' bg-label-info' }
            };
            if (typeof $status[$status_number] === 'undefined') {
              return data;
            }
            return (
              '<span class="badge ' + $status[$status_number].class + '">' + $status[$status_number].title + '</span>'
            );
          }
        },
        {
          // Actions
          targets: -1,
          title: 'Actions',
          orderable: false,
          searchable: false,
          render: function (data, type, full, meta) {
            return (
              '<div class="d-inline-block">' +
              '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="ti ti-dots-vertical ti-md"></i></a>' +
              '<div class="dropdown-menu dropdown-menu-end m-0">' +
              '<a href="javascript:;" class="dropdown-item">Details</a>' +
              '<a href="javascript:;" class="dropdown-item">Archive</a>' +
              '<div class="dropdown-divider"></div>' +
              '<a href="javascript:;" class="dropdown-item text-danger delete-record">Delete</a>' +
              '</div>' +
              '</div>' +
              '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon item-edit"><i class="ti ti-pencil ti-md"></i></a>'
            );
          }
        }
      ],
      language: {
        url: '//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/' + lang + '.json',
        paginate: {
          next: '<i class="ti ti-chevron-right ti-sm"></i>',
          previous: '<i class="ti ti-chevron-left ti-sm"></i>'
        }
      },
      order: [[2, 'desc']],
      displayLength: 7,
      dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end mt-n6 mt-md-0"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      lengthMenu: [7, 10, 25, 50, 75, 100],
      responsive: {
        details: {
          display: $.fn.dataTable.Responsive.display.modal({
            header: function (row) {
              var data = row.data();
              return 'Details of ' + data['full_name'];
            }
          }),
          type: 'column',
          renderer: function (api, rowIdx, columns) {
            var data = $.map(columns, function (col, i) {
              return col.title !== '' // ? Do not show row in modal popup if title is blank (for check box)
                ? '<tr data-dt-row="' +
                    col.rowIndex +
                    '" data-dt-column="' +
                    col.columnIndex +
                    '">' +
                    '<td>' +
                    col.title +
                    ':' +
                    '</td> ' +
                    '<td>' +
                    col.data +
                    '</td>' +
                    '</tr>'
                : '';
            }).join('');

            return data ? $('<table class="table"/><tbody />').append(data) : false;
          }
        }
      }
    });
  }

  // Filter form control to default size
  // ? setTimeout used for multilingual table initialization
  setTimeout(() => {
    $('.dataTables_filter .form-control').removeClass('form-control-sm');
    $('.dataTables_length .form-select').removeClass('form-select-sm');
  }, 300);
});
