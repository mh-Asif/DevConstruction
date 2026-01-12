/**
 * DataTables Basic
 */

'use strict';

let fv, offCanvasEl;
document.addEventListener('DOMContentLoaded', function (e) {
  (function () {
    const formAddNewRecord = document.getElementById('form-add-new-record'),
     select2 = $('.select2'),
    commentEditor = document.querySelector('.comment-editor'),
    datePicker = document.querySelector('#due-date'),
    startDatePicker = document.querySelector('#start-date');

       // Comment editor
  if (commentEditor) {
    new Quill(commentEditor, {
      modules: {
        toolbar: '.comment-toolbar'
      },
      placeholder: 'Write a Comment... ',
      theme: 'snow'
    });
  }
    // select2
  if (select2.length) {
    function renderLabels(option) {
      if (!option.id) {
        return option.text;
      }
      var $badge = option.text;
      return $badge;
    }

    select2.each(function () {
      var $this = $(this);
      $this.wrap("<div class='position-relative'></div>").select2({
        placeholder: 'Select Label',
        dropdownParent: $this.parent(),
        templateResult: renderLabels,
        templateSelection: renderLabels,
        escapeMarkup: function (es) {
          return es;
        }
      });
    });
  }
     // datepicker init
    if (datePicker) {
      datePicker.flatpickr({
        monthSelectorType: 'static',
        altInput: true,
        altFormat: 'j F, Y',
        dateFormat: 'Y-m-d',
          minDate: 0 
      });
    }
     // datepicker init
    if (startDatePicker) {
      startDatePicker.flatpickr({
        monthSelectorType: 'static',
        altInput: true,
        altFormat: 'j F, Y',
          dateFormat: 'Y-m-d',
          minDate: 0 
      });
    }

    setTimeout(() => {
      const newRecord = document.querySelector('.create-new'),
        offCanvasElement = document.querySelector('#add-new-task');

      // To open offCanvas, to add new record
      if (newRecord) {
        newRecord.addEventListener('click', function () {
          offCanvasEl = new bootstrap.Offcanvas(offCanvasElement);
            // Clear form fields when opening the offcanvas
          // offCanvasElement.querySelector('#taskName').value = '';
          // offCanvasElement.querySelector('#owner').value = '';
          // offCanvasElement.querySelector('#status').value = '';
          // offCanvasElement.querySelector('#tags').value = '';
          // offCanvasElement.querySelector('#startDate').value = '';
          // offCanvasElement.querySelector('#dueDate').value = '';
          // offCanvasElement.querySelector('#duration').value = '';
          // offCanvasElement.querySelector('#priority').value = 'Medium'; 
          // offCanvasElement.querySelector('#completed').value = 'No'; 
          // offCanvasElement.querySelector('#timelogTotal').value = '';

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
  var dt_basic_table = $('.task-datatable'),
    dt_complex_header_table = $('.dt-complex-header'),
    dt_row_grouping_table = $('.dt-row-grouping'),
    dt_multilingual_table = $('.dt-multilingual'),
    dt_basic;

  // DataTable with buttons
  // --------------------------------------------------------------------

    if (dt_basic_table.length) {
        dt_basic = dt_basic_table.DataTable({
            //  ajax: assetsPath + 'json/project-listing.json',

            //columns: [
            //  { data: '' }, // Empty column for actions or checkboxes
            //  { data: 'id' }, // The 'id' field in your data
            //  { data: 'id' }, // The 'id' field again, could be used for something else like an action
            //  { data: 'percentage' }, 

            //  { data: 'owner' }, // The 'owner' field in your data (instead of 'email')
            //  { data: 'status' }, // The 'status' field in your data
            //  { data: 'tasks' }, // The 'status' field in your data
            //  { data: 'phases' }, // The 'status' field in your data
            //  { data: 'start_date' }, // The 'start_date' field in your data
            //  { data: 'issues' }, // The 'start_date' field in your data
            //  { data: 'end_date' }, // The 'tags' field in your data
            //  { data: 'tags' }, // The 'tags' field in your data
            //  { data: 'tags' }, // The 'tags' field in your data

            //],
            //columnDefs: [
            //  {
            //    // For Responsive
            //    className: 'control',
            //    orderable: false,
            //    searchable: false,
            //    responsivePriority: 2,
            //    targets: 0,
            //    render: function (data, type, full, meta) {
            //      return '';
            //    }
            //  },
            //  {
            //    // Label
            //    targets: 5,
            //    render: function (data, type, full, meta) {
            //      var $status_number = full['status'];
            //      var $status = {
            //        1: { title: 'Completed', class: ' bg-label-success' },
            //        2: { title: 'Progress', class: ' bg-label-warning' },
            //        3: { title: 'Pending', class: ' bg-label-danger' },
            //      };
            //      if (typeof $status[$status_number] === 'undefined') {
            //        return data;
            //      }
            //      return (
            //        '<span class="badge ' + $status[$status_number].class + '">' + $status[$status_number].title + '</span>'
            //      );
            //    }
            //  },
            //  {
            //    // Label
            //    targets: 3,
            //    render: function (data, type, full, meta) {
            //      var $status_number = full['percentage'];
            //      return `
            //      <div class="d-flex align-items-center">
            //        <div class="progress w-100 me-3" style="height: 6px;">
            //          <div class="progress-bar ${$status_number < 50 ? 'bg-danger' : $status_number < 100 ? 'bg-warning' : 'bg-success'}" 
            //              style="width: ${$status_number}%;" 
            //              aria-valuenow="${$status_number}" 
            //              aria-valuemin="0" 
            //              aria-valuemax="100">
            //          </div>
            //        </div>
            //        <span class="text-heading">${$status_number}%</span>
            //      </div>
            //    `;

            //    }
            //  },
            //  {
            //    // Avatar image/badge, Name and post
            //    targets: 2,
            //    responsivePriority: 4,
            //    render: function (data, type, full, meta) {
            //      var $user_img = full['avatar'],
            //        $name = full['project_name']
            //      if ($user_img) {
            //        // For Avatar image
            //        var $output =
            //          '<img src="' + assetsPath + 'img/avatars/' + $user_img + '" alt="Avatar" class="rounded-circle">';
            //      } else {
            //        // For Avatar badge
            //        var stateNum = Math.floor(Math.random() * 6);
            //        var states = ['success', 'danger', 'warning', 'info', 'primary', 'secondary'];
            //        var $state = states[stateNum],
            //          $name = full['project_name'],
            //          $initials = $name.match(/\b\w/g) || [];
            //        $initials = (($initials.shift() || '') + ($initials.pop() || '')).toUpperCase();
            //        $output = '<span class="avatar-initial rounded-circle bg-label-' + $state + '">' + $initials + '</span>';
            //      }
            //      // Creates full output for row
            //      var $row_output = 
            //      '<div class="container d-flex ps-0 justify-content-between align-items-center">' +  // New container with flex and justify-content-between
            //          '<div class="d-flex justify-content-start align-items-center user-name">' +
            //              '<div class="d-flex flex-column">' +
            //                  '<span class="emp_name text-truncate">' +
            //                      $name + // The employee's name
            //                  '</span>' +

            //              '</div>' +
            //          '</div>' +
            //          '<div class="action-button hover-action-button ms-auto">' +  // Removed the button and replaced with icon
            //              '<a href="task-list.html" class="btn btn-outline-primary waves-effect">' +
            //                  '<i class="fa fa-eye me-1"></i> Access Project' + 
            //              '</a>' + 
            //          '</div>' +
            //      '</div>';

            //      return $row_output;
            //    }
            //  },
            //  {
            //    // Avatar image/badge, Name and post
            //    targets: 4,
            //    responsivePriority: 4,
            //    render: function (data, type, full, meta) {
            //      var $user_img = full['avatar'],
            //        $name = full['owner']
            //      if ($user_img) {
            //        // For Avatar image
            //        var $output =
            //          '<img src="' + assetsPath + 'img/avatars/' + $user_img + '" alt="Avatar" class="rounded-circle">';
            //      } else {
            //        // For Avatar badge
            //        var stateNum = Math.floor(Math.random() * 6);
            //        var states = ['success', 'danger', 'warning', 'info', 'primary', 'secondary'];
            //        var $state = states[stateNum],
            //          $name = full['owner'],
            //          $initials = $name.match(/\b\w/g) || [];
            //        $initials = (($initials.shift() || '') + ($initials.pop() || '')).toUpperCase();
            //        $output = '<span class="avatar-initial rounded-circle bg-label-' + $state + '">' + $initials + '</span>';
            //      }
            //      // Creates full output for row
            //      var $row_output = 
            //      '<div class="container d-flex ps-0 justify-content-between align-items-center">' +  // New container with flex and justify-content-between
            //          '<div class="d-flex justify-content-start align-items-center user-name">' +
            //              '<div class="avatar-wrapper">' +
            //                  '<div class="avatar me-2">' +
            //                      $output + // Assuming $output contains the avatar image or content
            //                  '</div>' +
            //              '</div>' +
            //              '<div class="d-flex flex-column">' +
            //                  '<span class="emp_name text-truncate">' +
            //                      $name + // The employee's name
            //                  '</span>' +

            //              '</div>' +
            //          '</div>' +

            //      '</div>';

            //      return $row_output;
            //    }
            //  },
            //  {
            //    responsivePriority: 1,
            //    targets: 4
            //  },
            //  {
            //    // Label
            //    targets: -2,
            //    render: function (data, type, full, meta) {
            //      var $status_number = full['status'];
            //      var $status = {
            //        1: { title: 'High Priority', class: ' bg-label-danger' },
            //        2: { title: 'Medium Priority', class: ' bg-label-warning' },
            //        3: { title: 'Low Priority', class: ' bg-label-success' },
            //      };
            //      if (typeof $status[$status_number] === 'undefined') {
            //        return data;
            //      }
            //      return (
            //        '<span class="badge ' + $status[$status_number].class + '">' + $status[$status_number].title + '</span>'
            //      );
            //    }
            //  },
            //  {
            //    // Actions
            //    targets: -1,
            //    title: 'Actions',
            //    orderable: false,
            //    visible: false,
            //    searchable: false,
            //    render: function (data, type, full, meta) {
            //      return (
            //        '<div class="d-inline-block">' +
            //        '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="ti ti-dots-vertical ti-md"></i></a>' +
            //        '<ul class="dropdown-menu dropdown-menu-end m-0">' +
            //        '<li><a href="javascript:;" class="dropdown-item">Details</a></li>' +
            //        '<li><a href="javascript:;" class="dropdown-item">Archive</a></li>' +
            //        '<div class="dropdown-divider"></div>' +
            //        '<li><a href="javascript:;" class="dropdown-item text-danger delete-record">Delete</a></li>' +
            //        '</ul>' +
            //        '</div>' +
            //        '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon item-edit"><i class="ti ti-pencil ti-md"></i></a>'
            //      );
            //    }
            //  }
            //],
            order: [[1, 'asc']],
            dom: '<"card-header flex-column py-2 flex-md-row"<"head-label text-center"><"dt-action-buttons text-end pt-6 pt-md-0"B>><"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end mt-n6 mt-md-0"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 7,
            lengthMenu: [7, 10, 25, 50, 75, 100],
            language: {
                paginate: {
                    next: '<i class="ti ti-chevron-right ti-sm"></i>',
                    previous: '<i class="ti ti-chevron-left ti-sm"></i>'
                }
            },
            buttons: [
                {
                    extend: 'collection',
                    className: 'btn btn-label-primary dropdown-toggle me-4 waves-effect waves-light border-none',
                    text: '<i class="ti ti-file-export ti-xs me-sm-1"></i> <span class="d-none d-sm-inline-block">Export</span>',
                    buttons: [
                        {
                            extend: 'print',
                            text: '<i class="ti ti-printer me-1" ></i>Print',
                            className: 'dropdown-item',
                            exportOptions: {
                                columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
                                // prevent avatar to be display
                                format: {
                                    body: function (inner, coldex, rowdex) {
                                        if (inner.length <= 0) return inner;
                                        var el = $.parseHTML(inner);
                                        var result = '';
                                        $.each(el, function (index, item) {
                                            if (item.classList !== undefined && item.classList.contains('user-name')) {
                                                result = result + item.lastChild.firstChild.textContent;
                                            } else if (item.innerText === undefined) {
                                                result = result + item.textContent;
                                            } else result = result + item.innerText;
                                        });
                                        return result;
                                    }
                                }
                            },
                            customize: function (win) {
                                //customize print view for dark
                                $(win.document.body)
                                    .css('color', config.colors.headingColor)
                                    .css('border-color', config.colors.borderColor)
                                    .css('background-color', config.colors.bodyBg);
                                $(win.document.body)
                                    .find('table')
                                    .addClass('compact')
                                    .css('color', 'inherit')
                                    .css('border-color', 'inherit')
                                    .css('background-color', 'inherit');
                            }
                        },
                        {
                            extend: 'csv',
                            text: '<i class="ti ti-file-text me-1" ></i>Csv',
                            className: 'dropdown-item',
                            exportOptions: {
                                columns: [3, 4, 5, 6, 7],
                                // prevent avatar to be display
                                format: {
                                    body: function (inner, coldex, rowdex) {
                                        if (inner.length <= 0) return inner;
                                        var el = $.parseHTML(inner);
                                        var result = '';
                                        $.each(el, function (index, item) {
                                            if (item.classList !== undefined && item.classList.contains('user-name')) {
                                                result = result + item.lastChild.firstChild.textContent;
                                            } else if (item.innerText === undefined) {
                                                result = result + item.textContent;
                                            } else result = result + item.innerText;
                                        });
                                        return result;
                                    }
                                }
                            }
                        },
                        {
                            extend: 'excel',
                            text: '<i class="ti ti-file-spreadsheet me-1"></i>Excel',
                            className: 'dropdown-item',
                            exportOptions: {
                                columns: [3, 4, 5, 6, 7],
                                // prevent avatar to be display
                                format: {
                                    body: function (inner, coldex, rowdex) {
                                        if (inner.length <= 0) return inner;
                                        var el = $.parseHTML(inner);
                                        var result = '';
                                        $.each(el, function (index, item) {
                                            if (item.classList !== undefined && item.classList.contains('user-name')) {
                                                result = result + item.lastChild.firstChild.textContent;
                                            } else if (item.innerText === undefined) {
                                                result = result + item.textContent;
                                            } else result = result + item.innerText;
                                        });
                                        return result;
                                    }
                                }
                            }
                        },
                        {
                            extend: 'pdf',
                            text: '<i class="ti ti-file-description me-1"></i>Pdf',
                            className: 'dropdown-item',
                            exportOptions: {
                                columns: [3, 4, 5, 6, 7],
                                // prevent avatar to be display
                                format: {
                                    body: function (inner, coldex, rowdex) {
                                        if (inner.length <= 0) return inner;
                                        var el = $.parseHTML(inner);
                                        var result = '';
                                        $.each(el, function (index, item) {
                                            if (item.classList !== undefined && item.classList.contains('user-name')) {
                                                result = result + item.lastChild.firstChild.textContent;
                                            } else if (item.innerText === undefined) {
                                                result = result + item.textContent;
                                            } else result = result + item.innerText;
                                        });
                                        return result;
                                    }
                                }
                            }
                        },
                        {
                            extend: 'copy',
                            text: '<i class="ti ti-copy me-1" ></i>Copy',
                            className: 'dropdown-item',
                            exportOptions: {
                                columns: [3, 4, 5, 6, 7],
                                // prevent avatar to be display
                                format: {
                                    body: function (inner, coldex, rowdex) {
                                        if (inner.length <= 0) return inner;
                                        var el = $.parseHTML(inner);
                                        var result = '';
                                        $.each(el, function (index, item) {
                                            if (item.classList !== undefined && item.classList.contains('user-name')) {
                                                result = result + item.lastChild.firstChild.textContent;
                                            } else if (item.innerText === undefined) {
                                                result = result + item.textContent;
                                            } else result = result + item.innerText;
                                        });
                                        return result;
                                    }
                                }
                            }
                        }
                    ]
                },
                {
                    text: '<i class="ti ti-plus me-sm-1"></i> <span class="d-none d-sm-inline-block">Add New Task</span>',
                    className: 'create-new btn btn-primary waves-effect waves-light'
                }
            ],
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
            },
            initComplete: function (settings, json) {
                $('.card-header').after('<hr class="my-0">');
            }
        });
        $('div.head-label').html('<h5 class="card-title mb-0">All Task</h5>');
    }

  //  For Datatable
  // --------------------------------------------------------------------


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
  $('.task-datatable tbody').on('click', '.delete-record', function () {
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
