var queries = [
    {
        context: 'mobile',
        match: function() {
            //console.log('Mobile callback. Maybe hook up some tel: numbers?');
            // Your mobile specific logic can go here. 
        },
        unmatch: function() {
            // We're leaving mobile.    
        }
    },
    {
        context: 'phone-portrait',
        match: function() {
            //console.log('entering phone portrait');
            $('body').addClass('w-phone-portrait');
        },
        unmatch: function() {
            //console.log('leaving phone portrait!');
            $('body').removeClass('w-phone-portrait');
        }

    },
    {
        context: 'phone-landscape',
        match: function() {
            //console.log('entering phone landscape.');
            $('body').addClass('w-phone-landscape');
        },
        unmatch: function() {
            //console.log('leaving phone landscape!');
            $('body').removeClass('w-phone-landscape');
        }

    },
    {
        context: 'tablet-portrait',
        match: function() {
            //console.log('entering tablet portrait.');
            $('body').addClass('w-tablet-portrait');
            
        },
        unmatch: function() {
            //console.log('leaving tablet portrait!');
            $('body').removeClass('w-tablet-portrait');
        }

    },
    {
        context: 'tablet-landscape',
        match: function() {
            //console.log('entering tablet landscape.');
            $('body').addClass('w-tablet-landscape');
        },
        unmatch: function() {
            //console.log('leaving tablet landscape!');
            $('body').removeClass('w-tablet-landscape');
        }

    },
    {
        context: 'desktop',
        match: function() {
            //console.log('entering desktop.');
            $('body').addClass('w-desktop');
        },
        unmatch: function() {
            //console.log('leaving desktop!');
            $('body').removeClass('w-desktop');
        }

    }
];
// Go!
MQ.init(queries);


// trace

// Collapsible messages list
    //hide message_body after the first one
    $(".message_list .message_body:gt(0)").hide();
    //hide message li after the 5th
    $(".message_list li:gt(4)").hide();
    //toggle message_body
    $(".message_head").click(function(){
        $(this).next(".message_body").slideToggle(500)
        return false;
    });
    //collapse all messages
    $(".collpase_all_message").click(function(){
        $(".message_body").slideUp(500)
        return false;
    });
    //show all messages
    $(".show_all_message").click(function(){
        $(this).hide()
        $(".show_recent_only").show()
        $(".message_list li:gt(4)").slideDown()
        return false;
    });
    //show recent messages only
    $(".show_recent_only").click(function(){
        $(this).hide()
        $(".show_all_message").show()
        $(".message_list li:gt(4)").slideUp()
        return false;
    });
    $('video').mediaelementplayer({
	success: function(media, node, player) {
		$('#' + node.id + '-mode').html('mode: ' + media.pluginType);
	}
    });

