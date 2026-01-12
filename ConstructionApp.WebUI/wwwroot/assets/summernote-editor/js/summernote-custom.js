$('#summernote').summernote({
    placeholder: 'Type Something....',
    minHeight: 150, // Minimum height
    maxHeight: null, // No maximum height
    codemirror: { 
        theme: 'monokai'
    },
    toolbar: [
        ['fontsize', ['fontsize']], 
        ['style', ['bold', 'italic', 'underline', 'clear']], // Corrected
        ['fontname', ['fontname']],
        ['color', ['color']], 
        ['para', ['ul', 'ol', 'paragraph']], 
        ['height', ['height']], 
        ['table', ['table']], 
        ['insert', ['link', 'picture', 'video']], 
        ['view', ['codeview', 'help']], 
    ],    
    fontNames: ['Arial', 'Courier New', 'Helvetica', 'Times New Roman', 'Verdana'], // Available fonts
    fontSizes: ['8', '10', '12', '14', '16', '18', '24', '36', '48'], // Font sizes
});


    function autoResizeSummernote() {
        let editor = $('.note-editable');
        editor.css('height', 'auto');  // Reset height
        editor.height(editor[0].scrollHeight);  // Set new height
    }