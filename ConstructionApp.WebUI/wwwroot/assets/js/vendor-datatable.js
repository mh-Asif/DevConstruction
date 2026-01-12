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
    startDatePicker = document.querySelector('#start-date')
    
    ;
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
        dateFormat: 'Y-m-d'
      });
    }
     // datepicker init
    if (startDatePicker) {
      startDatePicker.flatpickr({
        monthSelectorType: 'static',
        altInput: true,
        altFormat: 'j F, Y',
        dateFormat: 'Y-m-d'
      });
    }

    setTimeout(() => {
      const newRecord = document.querySelector('.create-new'),
        offCanvasElement = document.querySelector('#add-new-record');

      // To open offCanvas, to add new record
      if (newRecord) {
        newRecord.addEventListener('click', function () {
          window.location.href = "AddVendor";
          console.log("click on add user");
        });
      }
    }, 200);


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
   
    var dt_basic_table = $('.vendor-datatable'),
    dt_complex_header_table = $('.dt-complex-header'),
    dt_row_grouping_table = $('.dt-row-grouping'),
    dt_multilingual_table = $('.dt-multilingual'),
    dt_basic;

  // DataTable with buttons
  // --------------------------------------------------------------------

  if (dt_basic_table.length) {
    dt_basic = dt_basic_table.DataTable({
     // ajax: assetsPath + 'json/user-list.json',
      
      //columns: [
      //  { data: '' }, // Empty column for actions or checkboxes
      // /* { data: 'id' },*/ // The 'id' field in your data
      //  { data: 'id' }, // The 'id' field again, could be used for something else like an action
      //  { data: 'Designation' }, 
       
      //  { data: 'Location' }, // The 'Locationr' field in your data (instead of 'email')
      //  { data: 'DOB' }, // The 'status' field in your data
      //  { data: 'Manager' }, // The 'status' field in your data
      //  { data: 'HOD' }, // The 'HOD' field in your data
      //  { data: 'confirmation_period' }, // The 'start_date' field in your data
      //  { data: 'Employee_type' }, // The 'Employee_type' field in your data
      //  { data: 'status' }, // The 'tags' field in your data
      //  { data: 'form_status' }, // The 'tags' field in your data
      //  { data: 'action' }, // The 'tags' field in your data
        
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
      //    targets: -3,
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
      //    // Avatar image/badge, Name and post
      //    targets: 2,
      //    responsivePriority: 4,
      //    render: function (data, type, full, meta) {
      //      var $user_img = full['avatar'],
      //        $name = full['full_name']
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
      //    // Actions
      //    targets: -1,
      //    title: 'Actions',
      //    orderable: false,
      //    searchable: false,
      //    render: function (data, type, full, meta) {
      //      return (
      //        '<div class="d-inline-block">' +
      //        '<a href="javascript:;" class="btn btn-sm btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="ti ti-dots-vertical ti-md"></i></a>' +
      //        '<ul class="dropdown-menu dropdown-menu-end m-0">' +
      //        '<li><a href="add-user.html" class="dropdown-item">Edit</a></li>' +
      //        '<div class="dropdown-divider"></div>' +
      //        '<li><a href="javascript:;" class="dropdown-item text-danger delete-record">Delete</a></li>' +
      //        '</ul>' +
      //        '</div>' +
      //        '<a href="add-user.html" class="btn btn-sm btn-text-secondary rounded-pill btn-icon item-edit"><i class="ti ti-pencil ti-md"></i></a>'
      //      );
      //    }
      //  }
      //],
      order: [[1, 'asc']],
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
                //format: {
                //  body: function (inner, coldex, rowdex) {
                //    if (inner.length <= 0) return inner;
                //    var el = $.parseHTML(inner);
                //    var result = '';
                //    $.each(el, function (index, item) {
                //      if (item.classList !== undefined && item.classList.contains('user-name')) {
                //        result = result + item.lastChild.firstChild.textContent;
                //      } else if (item.innerText === undefined) {
                //        result = result + item.textContent;
                //      } else result = result + item.innerText;
                //    });
                //    return result;
                //  }
                //}
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
          text: '<i class="ti ti-plus me-sm-1"></i> <span class="d-none d-sm-inline-block">Add New User</span>',
          className: 'create-new btn btn-primary waves-effect waves-light'
        }
      ],
      //responsive: {
      //  details: {
      //    display: $.fn.dataTable.Responsive.display.modal({
      //      header: function (row) {
      //        var data = row.data();
      //        return 'Details of ' + data['full_name'];
      //      }
      //    }),
      //    type: 'column',
      //    renderer: function (api, rowIdx, columns) {
      //      var data = $.map(columns, function (col, i) {
      //        return col.title !== '' // ? Do not show row in modal popup if title is blank (for check box)
      //          ? '<tr data-dt-row="' +
      //              col.rowIndex +
      //              '" data-dt-column="' +
      //              col.columnIndex +
      //              '">' +
      //              '<td>' +
      //              col.title +
      //              ':' +
      //              '</td> ' +
      //              '<td>' +
      //              col.data +
      //              '</td>' +
      //              '</tr>'
      //          : '';
      //      }).join('');

      //      return data ? $('<table class="table"/><tbody />').append(data) : false;
      //    }
      //  }
      //},
      //initComplete: function (settings, json) {
      //  $('.card-header').after('<hr class="my-0">');
      //}
    });
  /*  $('div.head-label').html('<h5 class="card-title mb-0">All Users</h5>');*/
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
  $('.users-datatable tbody').on('click', '.delete-record', function () {
    dt_basic.row($(this).parents('tr')).remove().draw();
  });





  // Filter form control to default size
  // ? setTimeout used for multilingual table initialization
  setTimeout(() => {
    $('.dataTables_filter .form-control').removeClass('form-control-sm');
    $('.dataTables_length .form-select').removeClass('form-select-sm');
  }, 300);
});
