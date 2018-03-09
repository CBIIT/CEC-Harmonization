/**
 * @file
 * Behaviours for the Westat Nice menus module. This uses Superfish 1.4.8.
 *
 * @link http://users.tpg.com.au/j_birch/plugins/superfish
 */

(function ($) {

  /**
   * Add Superfish to all Westat Nice menus with some basic options.
   */
  Drupal.behaviors.niceMenus = {
    attach: function (context, settings) {
      $('ul.westat-nice-menu:not(.westat-nice-menus-processed)').addClass('westat-nice-menus-processed').each(function () {
        $(this).superfish({
          // Apply a generic hover class.
          hoverClass: 'over',
          // Disable generation of arrow mark-up.
          autoArrows: false,
          // Disable drop shadows.
          dropShadows: false,
          // Mouse delay.
          delay: Drupal.settings.westat_nice_menus_options.delay,
          // Animation speed.
          speed: Drupal.settings.westat_nice_menus_options.speed
        });

        // Add in Brandon Aaron’s bgIframe plugin for IE select issues.
        // http://plugins.jquery.com/node/46/release
        $(this).find('ul').bgIframe({opacity:false});

        $('ul.westat-nice-menu ul').css('display', 'none');
      });
    }
  };

})(jQuery);
