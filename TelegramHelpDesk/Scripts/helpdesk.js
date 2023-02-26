$(document).ready(function() {

    $.ajaxSetup({ cache: false });

    $(".openDialog").on("click",function (e) {
      
            e.preventDefault();

            $("<div></div>")
                .addClass("dialog")
                .attr("id",$(this)
                .attr("data-dialog-id"))
                .appendTo("body")
                .dialog({
                    title: $(this).attr("data-dialog-title"),
                    modal: true,
                    open: function(event,ui) {
                        $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").hide();
                        $(".ui-widget-overlay").click(function () {
                            $(".ui-dialog-titlebar-close").trigger('click');
                        });
                    }
                  })
                    .load(this.href);
    });
});
