/**
 *  Form Wizard
 */

'use strict';

// rateyo (jquery)
$(function () {
  var readOnlyRating = $('.read-only-ratings');

  

  // Wizard Checkout
  // --------------------------------------------------------------------

  const wizardCheckout = document.querySelector('#add-user');
  const wizardCheckoutvender = document.querySelector('#add-vender');



  // -add-user----- 

if (wizardCheckout) {
  const wizardCheckoutForm = wizardCheckout.querySelector('#add-user-form');
  if (!wizardCheckoutForm) return; // Exit if form not found

  // Wizard Steps
  const wizardCheckoutFormStep1 = wizardCheckoutForm.querySelector('#personal-infomation');
  const wizardCheckoutFormStep2 = wizardCheckoutForm.querySelector('#job-information');

  // Wizard Buttons
  const wizardCheckoutNext = Array.from(wizardCheckoutForm.querySelectorAll('.btn-next'));
  const wizardCheckoutPrev = Array.from(wizardCheckoutForm.querySelectorAll('.btn-prev'));

  let validationStepper = new Stepper(wizardCheckout, {
    linear: false
  });

  // Step 1 Validation
  const FormValidation1 = FormValidation.formValidation(wizardCheckoutFormStep1, {
    fields: {},
    plugins: {
      trigger: new FormValidation.plugins.Trigger(),
      bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '' }),
      autoFocus: new FormValidation.plugins.AutoFocus(),
      submitButton: new FormValidation.plugins.SubmitButton()
    }
  }).on('core.form.valid', function () {
    validationStepper.next();
  });

  // Step 2 Validation
  const FormValidation2 = FormValidation.formValidation(wizardCheckoutFormStep2, {
    fields: {},
    plugins: {
      trigger: new FormValidation.plugins.Trigger(),
      bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '' }),
      autoFocus: new FormValidation.plugins.AutoFocus(),
      submitButton: new FormValidation.plugins.SubmitButton()
    }
  }).on('core.form.valid', function () {
    alert('Form Submitted Successfully!');
  });

  // Next Button Click Event
  wizardCheckoutNext.forEach(item => {
    item.addEventListener('click', () => {
      if (validationStepper._currentIndex === 0) {
        FormValidation1.validate();
      } else if (validationStepper._currentIndex === 1) {
        FormValidation2.validate();
      }
    });
  });

  // Previous Button Click Event
  wizardCheckoutPrev.forEach(item => {
    item.addEventListener('click', () => {
      if (validationStepper._currentIndex > 0) {
        validationStepper.previous();
      }
    });
  });
}


// ----add-client--
// ----add-client--
const wizardCheckoutclient = document.querySelector('#add-client');

if (wizardCheckoutclient) {
  const wizardCheckoutForm = wizardCheckoutclient.querySelector('#add-client-form');
  if (!wizardCheckoutForm) return; // Exit if form not found

  // Wizard Steps
  const wizardCheckoutFormStep1 = wizardCheckoutForm.querySelector('#client-basic-details');
  const wizardCheckoutFormStep2 = wizardCheckoutForm.querySelector('#client-bussines-details');

  // Wizard Buttons
  const wizardCheckoutNext = Array.from(wizardCheckoutForm.querySelectorAll('.btn-next'));
  const wizardCheckoutPrev = Array.from(wizardCheckoutForm.querySelectorAll('.btn-prev'));

  // Ensure Stepper is initialized with the correct element
  let validationStepper = new Stepper(wizardCheckoutclient, {
    linear: false
  });

  // Step 1 Validation
  const FormValidation1 = FormValidation.formValidation(wizardCheckoutFormStep1, {
    fields: {}, // Define validation rules here
    plugins: {
      trigger: new FormValidation.plugins.Trigger(),
      bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '' }),
      autoFocus: new FormValidation.plugins.AutoFocus(),
      submitButton: new FormValidation.plugins.SubmitButton()
    }
  });

  // Step 2 Validation
  const FormValidation2 = FormValidation.formValidation(wizardCheckoutFormStep2, {
    fields: {}, // Define validation rules here
    plugins: {
      trigger: new FormValidation.plugins.Trigger(),
      bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '' }),
      autoFocus: new FormValidation.plugins.AutoFocus(),
      submitButton: new FormValidation.plugins.SubmitButton()
    }
  });

  // Next Button Click Event with Proper Validation
  wizardCheckoutNext.forEach(item => {
    item.addEventListener('click', (event) => {
      event.preventDefault(); // Prevent default form submission behavior
      let currentIndex = validationStepper.getActiveStepIndex();

      if (currentIndex === 0) {
        FormValidation1.validate().then(status => {
          if (status === 'Valid') validationStepper.next();
        });
      } else if (currentIndex === 1) {
        FormValidation2.validate().then(status => {
          if (status === 'Valid') {
            alert('Form Submitted Successfully!');
          }
        });
      }
    });
  });

  // Previous Button Click Event
  wizardCheckoutPrev.forEach(item => {
    item.addEventListener('click', () => {
      if (validationStepper.getActiveStepIndex() > 0) {
        validationStepper.previous();
      }
    });
  });
}



// -add-vender----- 


  const wizardCheckoutbackup = document.querySelector('#add-user-backup');
  if (typeof wizardCheckoutbackup !== undefined && wizardCheckoutbackup !== null) {
    // Wizard form
    const wizardCheckoutbackupForm = wizardCheckoutbackup.querySelector('#add-user-form');
    // Wizard steps
    const wizardCheckoutbackupFormStep1 = wizardCheckoutbackupForm.querySelector('#personal-infomation');
    const wizardCheckoutbackupFormStep2 = wizardCheckoutbackupForm.querySelector('#job-information');
    const wizardCheckoutbackupFormStep3 = wizardCheckoutbackupForm.querySelector('#contact-detail');
    const wizardCheckoutbackupFormStep4 = wizardCheckoutbackupForm.querySelector('#academic');
    // Wizard next prev button
    const wizardCheckoutbackupNext = [].slice.call(wizardCheckoutbackupForm.querySelectorAll('.btn-next'));
    const wizardCheckoutbackupPrev = [].slice.call(wizardCheckoutbackupForm.querySelectorAll('.btn-prev'));

    let validationStepper = new Stepper(wizardCheckoutbackup, {
      linear: false
    });

    // Cart
    const FormValidation1 = FormValidation.formValidation(wizardCheckoutbackupFormStep1, {
      fields: {
        // * Validate the fields here based on your requirements
      },

      plugins: {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
          // Use this for enabling/changing valid/invalid class
          // eleInvalidClass: '',
          eleValidClass: ''
          // rowSelector: '.col-lg-6'
        }),
        autoFocus: new FormValidation.plugins.AutoFocus(),
        submitButton: new FormValidation.plugins.SubmitButton()
      }
    }).on('core.form.valid', function () {
      // Jump to the next step when all fields in the current step are valid
      validationStepper.next();
    });

    // Address
    const FormValidation2 = FormValidation.formValidation(wizardCheckoutbackupFormStep2, {
      fields: {
        // * Validate the fields here based on your requirements
      },
      plugins: {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
          // Use this for enabling/changing valid/invalid class
          // eleInvalidClass: '',
          eleValidClass: ''
          // rowSelector: '.col-lg-6'
        }),
        autoFocus: new FormValidation.plugins.AutoFocus(),
        submitButton: new FormValidation.plugins.SubmitButton()
      }
    }).on('core.form.valid', function () {
      // Jump to the next step when all fields in the current step are valid
      validationStepper.next();
    });

    // Payment
    const FormValidation3 = FormValidation.formValidation(wizardCheckoutbackupFormStep3, {
      fields: {
        // * Validate the fields here based on your requirements
      },
      plugins: {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
          // Use this for enabling/changing valid/invalid class
          // eleInvalidClass: '',
          eleValidClass: ''
          // rowSelector: '.col-lg-6'
        }),
        autoFocus: new FormValidation.plugins.AutoFocus(),
        submitButton: new FormValidation.plugins.SubmitButton()
      }
    }).on('core.form.valid', function () {
      validationStepper.next();
    });

    // Confirmation
    const FormValidation4 = FormValidation.formValidation(wizardCheckoutbackupFormStep4, {
      fields: {
        // * Validate the fields here based on your requirements
      },
      plugins: {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
          // Use this for enabling/changing valid/invalid class
          // eleInvalidClass: '',
          eleValidClass: '',
          rowSelector: '.col-md-12'
        }),
        autoFocus: new FormValidation.plugins.AutoFocus(),
        submitButton: new FormValidation.plugins.SubmitButton()
      }
    }).on('core.form.valid', function () {
      // You can submit the form
      // wizardCheckoutbackupForm.submit()
      // or send the form data to server via an Ajax request
      // To make the demo simple, I just placed an alert
      alert('Submitted..!!');
    });

    wizardCheckoutbackupNext.forEach(item => {
      item.addEventListener('click', event => {
        // When click the Next button, we will validate the current step
        switch (validationStepper._currentIndex) {
          case 0:
            FormValidation1.validate();
            break;

          case 1:
            FormValidation2.validate();
            break;

          case 2:
            FormValidation3.validate();
            break;

          case 3:
            FormValidation4.validate();
            break;

          default:
            break;
        }
      });
    });

    wizardCheckoutbackupPrev.forEach(item => {
      item.addEventListener('click', event => {
        switch (validationStepper._currentIndex) {
          case 3:
            validationStepper.previous();
            break;

          case 2:
            validationStepper.previous();
            break;

          case 1:
            validationStepper.previous();
            break;

          case 0:

          default:
            break;
        }
      });
    });
  }
})();
