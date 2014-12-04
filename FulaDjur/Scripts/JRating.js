
PostRating = function (v, bildid) {
    $.ajax({
        type: "POST",
        url: "/Rating/AddRatingNumbers",
        data: { number: v, bildId: bildid },
        success: function () {
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("nu blev det fel");
        }
    });
};
(function ($) {
   
    $.fn.rating = function(options) {
        var settings = $.extend(
        {
            rateEnd: function(value) {}
        }, options);

        function setRating(e, ul) {
            var i = parseInt(e.val());
            
          
            if (!i) {
                i = 0;
            }

            ul.find('a').removeAttr('class');
            ul.find('a:lt(' + i + ')').attr('class', 'full');
        }

        this.each(function() {
            var e = $(this);
            var c = parseInt(e.attr("class").match(/rating\d+/)[0].replace('rating', ''));

            var ul = $('<ul class="rating"></ul>').insertAfter(e).width(c * 20 + 'px');

            if (c > 0) {
                for (k = 0; k < c; k++) {
                    ul.append('<li><a href="javascript:void(0);" title="' + (k + 1) + '">' + (k + 1) + '</a></li>');
                }
            }

            if (e.prop('readonly')) {
                var i = parseInt(e.val());

                if (!i) {
                    i = 0;
                }

                ul.find('a').attr('title', i + ' / ' + c);
            } else {
                ul.find('a').each(function(index, link) {
                    var link = $(link);

                    link.hover(function() {
                        ul.find('a').removeAttr('class');
                        ul.find('a:lt(' + (index + 1) + ')').attr('class', 'hover');

                    }, function() {
                        setRating(e, ul);
                    });

                    link.click(function () {
                      var bildid = $(this).parent().parent().parent().find('#djur_Id').val();
                        e.val(index + 1);
                        var v = parseInt(e.val());
                        PostRating(v, bildid);
                    
                        setRating(e, ul);

                        settings.rateEnd(index +1);
                    });
                });
            }

            setRating(e, ul);

            e.hide();
        });

        return this;
    };

})(jQuery);
