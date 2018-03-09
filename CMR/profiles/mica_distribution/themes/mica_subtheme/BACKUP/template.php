<?php


/**
 * Implements hook_bootstrap_based_theme().
 */
function mica_subtheme_bootstrap_based_theme() {
  return array('mica_subtheme' => TRUE);
}

/**
 * Implements hook_form_alter().
 */
function mica_subtheme_form_alter(&$form, &$form_state, $form_id) {
  if ($form_id == 'search_block_form') {
    $form['search_block_form']['#title'] = t('Search'); // Change the text on the label element
  }
  if ($form_id == 'user_login_block') {
    $form['links']['#weight'] = 100;
  }
}

/**
 * Adds the search form's submit button right after the input element.
 *
 * @ingroup themable
 */
function mica_subtheme_bootstrap_search_form_wrapper(&$variables) {
  $output = '<div class="input-append">';
  $output .= '<label class="element-invisible" for="' . $variables['element']['#id'] . '">';
  $output .= $variables['element']['#title'];
  $output .= '</label>';
  $output .= $variables['element']['#children'];
  $output .= '<button type="submit" class="btn">';
  $output .= '<i class="icon-search"></i>';
  $output .= '<span class="element-invisible">' . t('Search') . '</span>';
  $output .= '</button>';
  $output .= '</div>';
  return $output;
}
 
/*
 * Implements hook_html_head_alter
 */
function mica_subtheme_html_head_alter(&$head_elements) {
  // Force the latest IE rendering engine and Google Chrome Frame.
  $head_elements['chrome_frame'] = array(
    '#type' => 'html_tag',
    '#tag' => 'meta',
    '#attributes' => array('http-equiv' => 'X-UA-Compatible', 'content' => 'IE=edge,chrome=1'),
  );
}

function mica_subtheme_menu_link(array $variables) {
  $element = $variables['element'];
  $sub_menu = '';
  
  if ($element['#below']) {

    // Prevent dropdown functions from being added to management menu as to not affect navbar module.
    if (($element['#original_link']['menu_name'] == 'management') && (module_exists('navbar'))) {
      $sub_menu = drupal_render($element['#below']);
    }

    else {
      // Add our own wrapper
      unset($element['#below']['#theme_wrappers']);
      $sub_menu = drupal_render($element['#below']);
      //$element['#localized_options']['attributes']['class'][] = 'dropdown-toggle';
      //$element['#localized_options']['attributes']['data-toggle'] = 'dropdown';

      // Check if this element is nested within another
      if ((!empty($element['#original_link']['depth'])) && ($element['#original_link']['depth'] > 1)) {
        // Generate as dropdown submenu
        //$element['#attributes']['class'][] = 'dropdown-submenu';
      }
      else {
        // Generate as standard dropdown
        //$element['#attributes']['class'][] = 'dropdown';
        //$element['#localized_options']['html'] = TRUE;
        //$element['#title'] .= ' <span class="caret"></span>';
      }

      // Set dropdown trigger element to # to prevent inadvertant page loading with submenu click
      //$element['#localized_options']['attributes']['data-target'] = '#';
    }
  }
 // Issue #1896674 - On primary navigation menu, class 'active' is not set on active menu item.
 // @see http://drupal.org/node/1896674
 if (($element['#href'] == $_GET['q'] || ($element['#href'] == '<front>' && drupal_is_front_page())) && (empty($element['#localized_options']['language']) || $element['#localized_options']['language']->language == $language_url->language)) {
   $element['#attributes']['class'][] = 'active';
 }
  $output = l($element['#title'], $element['#href'], $element['#localized_options']);
  return '<li' . drupal_attributes($element['#attributes']) . '>' . $output . $sub_menu . "</li>\n";
}