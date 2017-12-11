﻿function displayMessage(message) {
  $("#status-message").html(message);
  $("#status-message").animate({top:'60px'}, 1000);
  $("#status-message").animate({top:'60px'}, 2000);
  $("#status-message").animate({top:'-150px'}, 500);
}

$("#submit-post-button").click(function(event) {
  event.preventDefault();
  var $form = $("#submit-post-form"),
    term = $form.find("textarea[name='Message']").val(),
    url = $form.attr("action");

  var model = { Message: term };

  var posting = $.post( url, model);

  posting.done(function(data) {
    displayMessage(data.message);
    $('#barkModal').modal( 'hide' );
    $('.bark-textarea').val('');
  })
})

$("#modal-submit-btn").click(function(event) {
  event.preventDefault();
  var $form = $("#modal-submit-post-form"),
  term = $form.find("textarea[name='Message']").val(),
  url = $form.attr("action");

  var model = { Message: term };

  var posting = $.post( url, model);

  posting.done(function(data) {
    displayMessage(data.message);
    $('#barkModal').modal( 'hide' );
    $('.modal-textarea').val('');
  })
})
