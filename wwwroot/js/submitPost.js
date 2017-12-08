$(".submit-post-form").click(function(event) {
    event.preventDefault();
    var $form = $(this),
        term = $form.find("textarea[name='Message']").val(),
        url = $form.attr("action");

    var model = { Message: term };

    var posting = $.post( url, model);

    posting.done(function(data) {
        console.log(data);
        $('#barkModal').modal( 'hide' )
    })
})
