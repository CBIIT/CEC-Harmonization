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
    $items = array();
    $items[] = l('New Account', 'user/register', array('attributes' => array(
                                                                          'title' => t('Create your CMR Account.'),
                                                                          'class' => array('regLink'))));
    $items[] = l('Request new password', 'user/password', array('attributes' => array(
                                                                                      'title' => t('Request new password via e-mail.'),
                                                                                      'class' => array('passLink'))));
    $items[] = l('Contact Us', 'content/contact-us', array('attributes' => array(
                                                                                      'title' => t('Contact Us.'),
                                                                                      'class' => array('contactUs'))));
    $items[] = l('User Guide', 'sites/default/files/CMRUserGuide.pdf', array('attributes' => array(
                                                                                      'title' => t('User Guide.'),
                                                                                      'class' => array('userGuide'))));
    $form['links']['#markup'] = theme('item_list', array('items' => $items));
  }
  if ($form_id == 'user_register_form') {
    unset($form['account']['mail']['#description']);
    unset($form['account']['pass']['#description']);
    //$form['account']['mail']['#placeholder'] = t('Email');
    //$form['account']['pass_pass1']['#placeholder'] = t('Password');
    //$form['profile_main']['field_profile_first_name']['#placeholder'] = t('First Name');
    //$form['account']['profile_main[field_profile_first_name][und][0][value]']['#placeholder'] = t('Last Name');
    $form['actions']['submit']['#value'] = t('Register');
    $form['actions']['submit']['#attributes']['class'][] = 'btn btn-primary';
    $form['actions']['cancel'] = array(
      '#type' => 'button',
      '#value' => t('Cancel'),
      '#weight' => -1,
      '#attributes' => array('class' => array('cancel-go-home')),
     );
    $form['terms_of_use']['#description'] = 'Read the following researcher agreement thoroughly.
                                              By agreeing electronically, you acknowledge that you have both
                                              read and understood all text presented to you as part of the
                                              registration process.';
    //if (!empty($form_state['clicked_button']['#value']) && $form_state['clicked_button']['#id'] == 'cancel') {
    //  return false;
    //  drupal_goto('home');
    //}
  }
  
  if ($form_id == 'webform_client_form_10774' || $form_id == 'webform_client_form_10776') { // contact us and request form
    $form['actions']['cancel'] = array(
      '#type' => 'button',
      '#value' => t('Cancel'),
      '#weight' => -1,
      '#attributes' => array('class' => array('cancel-go-home')),
      //'#callback' => drupal_goto('/'),
     );
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
      //$element['#below']['#attributes']['tabindex'] = -1;
      //$sub_menu = '<ul role="menu" aria-hidden="true">' . drupal_render($element['#below']) . '</ul>';
      $sub_menu = drupal_render($element['#below']);
     // $element['#localized_options']['attributes']['class'][] = 'dropdown-toggle';
      //$element['#localized_options']['attributes']['data-toggle'] = 'dropdown';
      //$element['#attributes']['tabindex'] = 0;
      //$element['#attributes']['aria-haspopup'] = "true";

      // Check if this element is nested within another
      if ((!empty($element['#original_link']['depth'])) && ($element['#original_link']['depth'] > 1)) {
        // Generate as dropdown submenu
        //$element['#attributes']['class'][] = 'dropdown-submenu';
        //$element['#attributes']['tabindex'] = -1;
        //unset($element['#attributes']['aria-haspopup']);
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

/**
 * Overrides theme_fieldset().
 *
 * Add another div wrapper around fieldsets for styling purposes.
 */
function mica_subtheme_fieldset($variables) {
  $element = $variables['element'];
  element_set_attributes($element, array('id'));
  _form_set_class($element, array('form-wrapper'));

  $output = '<fieldset' . drupal_attributes($element['#attributes']) . '>';
  if (!empty($element['#title'])) {
    // Always wrap fieldset legends in a SPAN for CSS positioning.
    if ($element['#title'] == t('CAPTCHA')) {
    	$output .= '<legend><span class="fieldset-legend">' . t('Security') . '</span></legend>';
  	 } else {
    	$output .= '<legend><span class="fieldset-legend">' . $element['#title'] . '</span></legend>';
    }
  }
  $output .= '<div class="fieldset-wrapper">';
  if (!empty($element['#description'])) {
    $output .= '<div class="fieldset-description">' . $element['#description'] . '</div>';
  }
  $output .= $element['#children'];
  if (isset($element['#value'])) {
    $output .= $element['#value'];
  }
  $output .= '</div>';
  $output .= "</fieldset>\n";
  return $output;
}

/**
 * Implement hook_mail_alter().
 */
//function mica_subtheme_logintoboggan_mail_alter_alter(&$message) {
//  //if ($message['id'] == 'user_register_pending_approval_admin') {
//  //  $reg_pass_set = !variable_get('user_email_verification', TRUE);
//  //  if ($reg_pass_set) {
//  //    $account = $message['params']['account'];
//  //    $url_options = array('absolute' => TRUE);
//  //    $language = $message['language'];
//  //    $langcode = isset($language) ? $language->language : NULL;
//  //    $message['body'][] = t("Helloooo");
//  //  }
//  //}
//}