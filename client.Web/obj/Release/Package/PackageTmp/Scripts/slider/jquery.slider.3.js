jQuery(document).ready(function($) {    
    /**
    * Add hovers to the navigation, and other items.
    *
    * @since 1.0.0
    */
    /*$( '#navigation a' ).hover(function() {
    var original = $( this ).children().children( 'img' ).attr( 'src' ).split( '_n.' );
    $( this ).children().children( 'img' ).attr( 'src', original[0] + '_h.' + original[1] );
    }, function() {
    var original = $( this ).children().children( 'img' ).attr( 'src' ).split( '_h' );
    $( this ).children().children( 'img' ).attr( 'src', original[0] + '_n' + original[1] );
    });*/

    $('#slide-scroller .items .item').live('hover', function() {
        $(this).toggleClass('hover');
    }, function() {
        $(this).removeClass('hover');
    });

    /**
    * Create the scrollers.
    *
    * @since 1.0.0
    */
    $('#progress-scroll').scrollable({
        item: 'div',
        size: 1,
        circular: true,
        clickable: false,
        next: '.nextPage',
        prev: '.prevPage'
    }).navigator().autoscroll({ interval: 8000 });

    $('#slide-scroll').scrollable({
        circular: true,
        next: '.nextPage',
        prev: '.prevPage'
    }).navigator().autoscroll({ interval: 8000 });

    $('#slide-scroll .item:nth-child(3)').clone().addClass('cloned').appendTo('#slide-scroll .items');

    /*$('#tweeted').scrollable({
        item: 'ul',
        size: 1,
        speed: 500,
        circular: true,
        next: '.nextPage',
        prev: '.prevPage'
    }).navigator().autoscroll({ interval: 30000 });*/

   /* $('.prevPage, .nextPage').click(function() { return false; });

    jQuery(".ago").timeago();

    $('#iktldy-iktldy').focus();*/

});

