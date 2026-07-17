insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('About',1,1,0,0,1,GETDATE(),'ABT',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('About the project','<h4>About the project</h4>
<p>Having an abnormal BRCA gene is an inherited condition associated with an increased risk of cancer. This dedicated app for carriers of the BRCA gene has been developed to support women and men who have been diagnosed as carriers of the BRCA1 or BRCA2 gene.<br/> Development of the app has been supported and co-developed by Leicester Clinical Genetics Department and the NHS East Genomic Medicine Service Alliance.</p>
<p>We believe carriers of rare conditions often struggle to access timely information advice and that these types of digital aids can be helpful to patients to participate directly to prevent complications and personalise their health and well-being plans.</p>
<p class="abt_p_margin">The BRCA app is a secure app provided for you as a BRCA gene carrier to support you, with your consent.</p>
<ul>
<li>There is no requirement for you to use the app if you do not wish to and your NHS care will be unchanged whether you use the app or not.</li>
<li>The app is held and controlled by the patient.</li>
<li>All actions require explicit patient consent.</li>
<li>Your data is held confidentially and will not be shared with anyone without your consent.</li>
<li>All accesses to your patient-identifiable data are with patient consent.</li>
<li>You may withdraw your consent at any time by selecting &ldquo;No&rdquo; in the Consent section in the app.</li>
<li>The app is compliant with UK Government data protection regulations.</li>
</ul>
<p>The project aim is that the app will be rolled out nationally for all carriers of BRCA gene carrier to help patients navigate the NHS on a day to day basis and improve the care of their condition.</p>',1,2,(select Id from InformativePages where PageCode='ABT' and QuestionnaireId=1),0,1,GETDATE(),'ABTPRJ',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('About the app','<h4>About the app</h4>
<p>The app includes the following features for you</p>
<ul>
<li>An overview of the condition for your treating clinicians with a gene specific guideline.</li>
<li>A summary dashboard which in a single view can demonstrate whether screening is up  to date, lifestyle and other risk reducing options have been discussed as well as family planning options and cascading of risk information to relatives.</li>
<li>A summary letter for your relatives so they can access clinical genetics services via their general practitioner.</li>
<li>Links to support groups and decision aid tools to help you consider options such as mastectomy and salpingectomy which can lower the risk of developing cancer.</li>
<li>Screening updates, alerts and  reminders.</li>
<li>A guide on who to contact and what to do if you have certain “Red Flag” symptoms that could be suggestive of breast or ovarian cancer.</li>
<li>Updates on recommended management of being a BRCA carrier.</li>
<li>Updates on nationally approved clinical research studies.</li>
<li>Updates on approved BRCA gene carrier support days and educational events.</li>
<li>An opportunity to liaise directly with the app development team if you would like to make suggestions to improve its utility in navigating the NHS on a day to day basis and agree with consent to use the data generated to audit improvements in BRCA gene carrier management.</li>
</ul>',1,3,(select Id from InformativePages where PageCode='ABT' and QuestionnaireId=1),0,1,GETDATE(),'ABTAPP',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('How the app can help you','<h4>How the app can help you</h4>
<p>The App can help you in the following ways</p>
<ul>
<li><b>Generate a letter for you</b> to share with your GP explaining what it means to be a BRCA gene carrier and setting out the care you need.</li>
<li><b>Generate a letter for your family</b> explaining what it means to be a BRCA gene carrier and the steps your family can take to understand if they find out if they are BRCA gene carriers.</li>
<li><b>Learn more about</b> what it means to be a <b>BRCA gene carrier</b> to help you manage optimally.</li>
<li><b>Consider what to do</b> if you develop symptoms that might be due to BRCA .</li>
<li><b>Provide you with alerts and reminders</b> for the upcoming screening or management actions you need.</li>
<li><b>View your personal BRCA gene carrier</b> dashboard to see any issues that have not yet been discussed or outstanding screening appointments or management decisions.</li>
<li><b>Provide a summary</b> of your BRCA gene for the clinicians looking after you.</li>
<li><b>Receive clinical and research updates</b> in the future about the management of BRCA gene carriers.</li>
<li><b>Complete optional surveys</b> from time to time about your use of the app and suggestions for additions and changes you would like.</li>
</ul>',1,4,(select Id from InformativePages where PageCode='ABT' and QuestionnaireId=1),0,1,GETDATE(),'ABTHAH',1,1)

insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Consent',1,5,0,0,1,GETDATE(),'CNT',1,1)

insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Medical History',1,6,0,0,1,GETDATE(),'MDH',1,1)

insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Actions',1,7,0,0,1,GETDATE(),'ACT',1,1)

insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Alerts',1,8,0,0,1,GETDATE(),'ALT',1,1)

insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Personal dashboard',1,9,0,0,1,GETDATE(),'PND',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu,Gender)
values('Breast self-exam','Coming soon',1,10,0,0,1,GETDATE(),'BSE',1,1,'F')

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu,Gender)
values('Red flag symptoms','Coming soon',1,11,0,0,1,GETDATE(),'RFSM',1,1,'M')

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu,Gender)
values('Red flag symptoms','<div class="container">
<div class="redflag_header">
<p>Here are important symptoms to look out for and actions to take.</p>
<p>Although they may have other causes, they might be associated with Lynch Syndrome and may require further investigation.</p>
</div>
<p>For women only</p>
<p>Symptoms of possible breast cancer</p>
</div>
<div class="container">
<p class="rf_list_main">Lump in breast</p>
<p class="rf_list_sub">See GP for possible urgent<br />referral to breast team</p>
<!-- 1 data -->
<p class="rf_list_main">Change in shape of breast</p>
<p class="rf_list_sub">See GP for possible urgent <br />referral to breast team</p>
<p class="rf_list_main">Distortion of nipple</p>
<p class="rf_list_sub">See GP for possible urgent <br />referral to breast team</p>
<p class="rf_list_main">Eczema of nipple</p>
<p class="rf_list_sub">See GP for possible urgent <br />referral to breast team</p>
<p class="rf_list_main">Bleeding from nipple</p>
<p class="rf_list_sub">See GP for possible urgent <br />referral to colorectal team</p>
<p>Symptoms of possible ovarian cancer  </p>
  <hr/>
  <p class="rf_list_main">Abdominal distension</p>
<p class="rf_list_sub">See GP for possible urgent <br />referral to colorectal team</p>
<p class="rf_list_main">Abdominal pain or discomfort that is not usual for you</p>
<p class="rf_list_sub">See GP for possible urgent <br />referral to colorectal team</p>
<p class="rf_list_main">Heartburn / Gastro-oesophageal reflux every day for a week Tired all the time</p>
<p class="rf_list_sub">Seek advice from GP <br/>Seek advice from GP to check blood count </p>
<p class="rf_list_main">Recurrent headaches, particularly if worse in the morning and associated with nausea</p>
<p class="rf_list_sub">Seek advice from GP</p>
<p class="rf_list_main">Back pain that wakes you up at night not related to heavy lifting </p>
<p class="rf_list_sub">Seek advice from GP</p>
<p class="rf_list_main">For men only</p>
<p class="rf_list_sub">Symptoms of possible Prostate cancer</p>
<p class="rf_list_main">For men  -  Getting up two or more times at night to pass urine, unrelated to drinking alcohol</p>
<p class="rf_list_sub">Seek advice from GP</p>
</div>',1,12,0,0,1,GETDATE(),'RFSF',1,1,'F')

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('What is BRCA?','<p>What is BRCA?</p>
<div>BRCA stands for <strong>BR</strong>east <strong>CA</strong>ncer, referring to the genes BRCA1 and BRCA2, which are associated with an increased risk of breast and ovarian cancer. <br /> BRCA is an inherited condition usually caused by a significant change in one of two genes: BRCA1 or BRCA2. BRCA gene changes occur in both women and men.<br /> For women, BRCA is associated with a significantly increased risk of developing cancer of the breast or ovary. There is another gene called PALB2 which also increases the risk of these cancers. It predominantly affects women, increasing their chance of developing breast or ovarian cancer. <br /> For men, BRCA is associated with an increased chance of developing prostate cancer and, rarely, male breast cancer.</div>
<p>How does BRCA gene carrier affect families?</p>
<div>The gene can be passed on from generation to generation in what is called an autosomal dominant fashion. That means the gene may be inherited by half your children. Not everybody with the gene goes onto develop cancer but the majority will be affected without advice and support from their clinical team.</div>
<p>Risks</p>
<div>The lifetime risks of developing cancer are different depending on which gene is altered and whether you are male or female.<br /> Preventive surgery has been shown to lower the risk of cancer. Other changes in lifestyle that maintain weight and the body mass index in the normal range, not smoking and decreasing alcohol intake have also been shown to lower the risk of cancer.<br /> Screening has been shown to detect cancer at an earlier stage and improve long-term survival but not necessarily reduce the risk of developing cancer.</div>
<p>What can be done?</p>
<div>Lifestyle advice, regular breast screening and preventive surgery have been shown to reduce the chance of developing cancer. If you carry the BRCA gene you should be under the care of a specialist team and be able to seek advice if you have symptom concerns.</div>
<p>How do you find out about whether you carry this gene change if it is in your family?</p>
<div>Usually the condition is first detected due to tests been carried out on an affected family member.</div>
<p>Management of BRCA &ndash; What you can do</p>
<div>
<ul>
<li>Discussion of inheritance pattern of the condition, cascading of risk information and family planning options.</li>
<li>Discussion about diet, exercise and symptom awareness.</li>
<li>Discussion around breast screening from the age of 25.</li>
<li>Discussion about ovarian cancer risk and the possibility of preventative surgery for those over the age of 40.</li>
<li>Discussion around links to additional information, decision aids and support groups such as breast cancer charities, Macmillan.</li>
<li>Discussion about mental health and health behaviour.</li>
</ul>
</div>
<p>BRCA gene carrier and reproductive health advice</p>
<div>BRCA gene carrier is an inherited condition caused by a change in one of two genes involved in a process called DNA repair. If an individual inherits an altered copy of one of these genes from a parent they are potentially at a higher risk of cancer.<br /><br /> As we all inherit a copy of our genes from our mother and father, the chance of passing on an altered copy of a gene to our children is 50% or one in two for each pregnancy. This means that if you have an altered copy of a BRCA gene carrier gene, there is a one in two chance you could pass this onto a child, who could then be at a higher risk of cancer.<br /><br /> Some people are keen to avoid this and there are a number of options available for a couple:<br/>
  1) Have children naturally and hope the altered gene is not passed on. The child is usually offered testing over the age of 18 and advice given on cancer risk reducing or screening options.<br/>
  2) Use an egg or sperm donor that doesn''t have this altered gene.<br/>
  3) Adopt a child or not have children.<br/>
  4) Become pregnant in the usual fashion and carry out a needle based test in pregnancy such as a chorionic villous sampling or an amniocentesis to determine if the pregnancy is at risk. This is called a prenatal test but if the altered gene is detected it cannot be changed and a decision would need to be made as to whether to continue with the pregnancy.<br/>
5) Asked to be referred to a specialist fertility centre to consider a form of in vitro fertilisation (IVF) technology called pre-implantation genetic diagnosis (PGD) where an embryo is made through egg harvesting and sperm collection outside of the body and then tested at about an eight cell stage and only implanted into the womb of the female partner if the gene change has not been inherited. This is the most commonly used technique to avoid passing on BRCA gene carrier at this stage.
</div>
<p>What to do next</p>
<div>
If a relative of an individual with BRCA gene carrier over the age of 18 would like to discuss being tested, we recommend they take a copy of the GP letter (see Letter for GP in the app) to their GP and asked to be referred to their regional clinical genetics department to discuss this further.
</div>',1,13,0,0,1,GETDATE(),'WB',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Letter for GP','<p>BRCA gene carrier is an inherited condition usually caused by a significant change in one of two genes: BRCA 1 or 2. It is associated with a significantly increased risk of developing breast or ovarian cancer. </p>
<p>The genes are autosomal dominant so affect men and women equally, and their children have a 50% risk of being affected.  Close relatives should be informed and genetic counselling and testing made available to first degree relatives of affected family members over the age of 18 via their regional genetics department on receipt of a referral from their general practitioners. </p>
<p>Not everybody with the gene goes onto develop cancer but the majority will be affected without advice and support from their clinical team.</p>
<p><strong>Carriers of BRCA gene carrier should be under the care of a specialist team and be able to seek advice if they have symptom concerns.</strong></p>
<p><strong>Management of BRCA gene carrier should include: </strong></p>
<ul>
<li>MRI every year from the age of 25.</li>
<li>Mammogram every year from 40.</li>
<li>Discussion of inheritance pattern of the condition, cascading of risk information and family planning options.</li>
<li>For women, discussion on risk reducing mastectomy, discussion about gynaecological tumour risks and the possibility of preventive surgery for those over the age of 40.</li>
<li>Discussion about diet, exercise and symptom awareness.</li>
<li>Discussion about mental health and health behaviour.</li>
<li>Links to additional information, decision aids and support groups.</li>
<p>If a clinician would like additional information about BRCA gene it is available through your regional clinical genetics department.</p>
  <p>Links to national guidelines are also available here. </p>',1,14,0,0,1,GETDATE(),'LTG',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Letter for families','<p>BRCA gene carrier is an inherited condition caused by a significant change in one of two genes: BRCA 1 or 2. It is associated with a significantly increased risk of developing breast or ovarian cancer.<br /> The genes are autosomal dominant so affect men and women equally, and their children have a 50% risk of being affected. Close relatives should be informed and genetic counselling and testing made available to first degree relatives of affected family members over the age of 18 via their regional genetics department on receipt of a referral from their general practitioners.</p>
<p>Not everybody with the gene goes onto develop cancer but the majority will be affected without advice and support from their clinical team.</p>
<p><strong>Risks </strong></p>
<p>The lifetime risks of developing cancer are different depending on which gene is altered and whether you are male or female. Women are at risk of developing breast and/or ovarian cancer; men may rarely develop breast cancer and have an increased chance of developing prostate cancer.<br />Screening has been shown to detect tumours at any earlier stage and improve long-term survival but not necessarily reduce the risk of tumours.</p>
<p><strong>What can be done?</strong></p>
<p>Lifestyle advice, regular breast screening and risk reducing surgery have been shown to reduce the risk of disease and carriers of this condition should be under the care of a BRCA gene carrier specialist team and be able to seek advice if they have symptom concerns.</p>
<p><strong>How do you find out about whether you carry this gene change if it is in your family?</strong></p>
<p>Usually the condition is first detected due to tests been carried out on a stored tumour sample from an affected family member.</p>
<p><strong>Management of BRCA gene carrier should include:</strong></p>
<ul>
<ul>
<li>MRI every year from the age of 25.</li>
<li>Mammogram every year from 40.</li>
<li>Discussion of inheritance pattern of the condition, cascading of risk information and family planning options.</li>
<li>For women, discussion on risk reducing mastectomy, discussion about gynaecological tumour risks and the possibility of preventive surgery for those over the age of 40.</li>
<li>Discussion about diet, exercise and symptom awareness.</li>
<li>Discussion about mental health and health behaviour.</li>
<li>Links to additional information, decision aids and support groups.</li>
</ul>
</ul>
<p><strong>What to do next</strong></p>
<p>If a relative of an individual with BRCA gene carrier over the age of 18 would like to discuss being tested, we recommend they take a copy of the GP letter (see Letter for GP in the app) to their GP and asked to be referred to their regional clinical genetics department to discuss this further.</p>',1,15,0,0,1,GETDATE(),'LTF',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Helpful links','coming soon',1,16,0,0,1,GETDATE(),'HLK',1,1)

insert into InformativePages(Name,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('My details',1,17,0,0,1,GETDATE(),'MD',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Health Providers','coming soon',1,18,0,0,1,GETDATE(),'HP',1,1)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Terms And Conditions','<div class="container">  <div class="abt_project_div">  <div style="padding: 0px 5px 5px 5px;">  <h6>1.1 User Obligation</h6>  <p class="abt_p_margin" style="font-weight: 600;">Use of this app constitutes your acknowledgement and acceptance of these Terms and Conditions, which take effect on the date on which you first use the site. If these Terms and Conditions are not accepted in full, you do not have permission to access the contents of this website and therefore should cease using this website immediately.</p>  <h6>1.2 Copyright</h6>  <p class="abt_p_margin" style="font-weight: 600;">All copyright, trademarks and other intellectual property rights in Lynch Syndrome app (including the design, arrangement and look and feel) and all material or content supplied as part of Lynch Syndrome app shall remain at all times the property of Instant Access Medical Limited. In accessing the website, you agree that you do so only for your own personal, non-commercial use. You may not agree to, permit, or assist in any way any third party to copy, reproduce, download, post, store (including in any other web site), distribute, transmit, broadcast, commercially exploit or modify in any way the material or content or for any other purpose without Instant Access Medical Limited&rsquo;s prior written permission.</p>  <h6>1.3 Disclaimer</h6>  <p class="abt_p_margin" style="font-weight: 600;">The information contained in this Lynch Syndrome app is for medical, informational, and educational purposes only. While it is based on professional advice, published experience, and expert opinion, it does not represent a therapeutic recommendation or prescription. Consult your personal clinician for any specific medical information or for any medical treatment. The information contained in this site is provided on a blind-basis, without any knowledge of the reader''s medical condition, identity or specific circumstances. The application and impact of the information may vary from person to person. There may also be delays, omissions, or inaccuracies in information contained in this website. Instant Access Medical Limited is not responsible or liable for any diagnosis made by the user of Lynch Syndrome app. Instant Access Medical Limited will not be liable for any misdiagnosis on the part of clinician based on the records in the Lynch Syndrome app. The user takes all responsibility for accuracy and updating of the Lynch Syndrome app. Instant Access Medical Limited will not be liable for any errors caused by the correct/incorrect entries in the Lynch Syndrome app. The reminders Tab is to help the users to list their reminders. The user will be responsible for correctness /accuracy of reminders and Instant Access Medical Limited will not be liable for any inconvenience/errors caused. No user data is entered by Instant Access Medical Limited in the App. The User agrees to be responsible for the accuracy and updating of the Data entered in the App</p>  <h6>1.3 Limit of Liability</h6>  <p class="abt_p_margin" style="font-weight: 600;">Instant Access Medical Limited is not responsible for the availability or content of any third party websites or material you access through Lynch Syndrome app we do not endorse and are not responsible or liable for any content, advertising, products, services or information on or available from third party websites (including payment for and delivery of such products or services). Instant Access Medical Limited is not responsible for any damage, loss or offence caused by or, in connection with, any content, advertising, products, services or information available on such websites. Any terms, conditions, warranties or representations associated with such dealings, are solely between you and the relevant provider of the service.</p>  <h6>1.6 Links</h6>  <p class="abt_p_margin" style="font-weight: 600;">Links to Lynch Syndrome app must be direct to the page and must not be viewed within the pages of another site. Lynch Syndrome app disclaims all liability for any legal or other consequences (including for infringement of third party rights) of links made to Lynch Syndrome app</p>  <h6>1.7 Service Levels</h6>  <p class="abt_p_margin" style="font-weight: 600;">This site and the information, names, images, pictures, logos and icons relating to Lynch Syndrome app and/or any of Instant Access Medical Limited products and services (or to third party products and services), is provided "AS IS" and on an "AS AVAILABLE" basis without any representation or endorsement being made and without warranty of any kind, including but not limited to the implied warranties of satisfactory quality, fitness for a particular purpose, non-infringement, compatibility, security and accuracy. In no event will Instant Access Medical Limited and/or third parties be liable for any damages including, but not limited to, indirect or consequential damages or any damages including, but not limited to, errors or omissions, indirect or consequential damages or any damages whatsoever arising from use, loss of use, data, or profits, whether in action of contract, negligence, or other action, arising out of or in connection with the use of the site. Instant Access Medical Limited does not warrant that the functions contained in this site/app will be uninterrupted or error free or that defects will be corrected or that this site or the server that makes it available are free of viruses or bugs. Instant Access Medical Limited does not represent the full functionality, accuracy, or reliability of any material.</p>  <h6>1.8 Trade Marks</h6>  <p class="abt_p_margin" style="font-weight: 600;">The names, images and logos identifying Lynch Syndrome app or third parties are proprietary marks of Instant Access Medical Limited and/or third parties. Nothing contained herein shall be construed as conferring by implication, estoppel or otherwise any license or right under any trade mark or patent of Instant Access Medical Limited or any other third party.</p>  <h6>1.9 Indemnity</h6>  <p class="abt_p_margin" style="font-weight: 600;">You agree to indemnify and keep indemnified Instant Access Medical Limited and &lsquo;Lynch Syndrome app from and against all claims, damages, expenses, costs and liabilities arising in any manner from your entry to and use of Lynch Syndrome other than in accordance with these terms and conditions.</p>  <h6>1.10 Changes to Terms &amp; Conditions of Use</h6>  <p class="abt_p_margin" style="font-weight: 600;">Instant Access Medical Limited reserves the right to change these terms and conditions at any time by posting changes online and it is your responsibility to refer to and comply with these terms on accessing the site. Your continued use of this site after changes are posted constitutes your acceptance of these terms and conditions as modified.</p>  <h6>1.11 Law and Jurisdiction</h6>  <p class="abt_p_margin" style="font-weight: 600;">If any of these Terms and Conditions should be determined to be illegal, invalid, or otherwise unenforceable by reason of the laws of England in which these Terms and Conditions are intended to be effective, then to the extent and within the jurisdiction in which that Term or Condition is illegal, invalid, or unenforceable, it shall be severed and deleted from that clause and the remaining terms and conditions shall survive and continue to be binding and enforceable. These Terms and Conditions shall be governed by and construed in accordance with the laws of the England arising here from shall be exclusively subject to the jurisdiction of the courts of England.</p>  </div>  </div>  </div>',1,19,0,0,1,GETDATE(),'TNC',1,0)

insert into InformativePages(Name,Description,QuestionnaireId,SortOrder,ParentPageId,IsApplication,IsActive,CreatedOn,PageCode,LanguageId,IsMenu)
values('Privacy Policy','<div class="container">  <div class="abt_project_div">  <div style="padding: 0px 5px 5px 5px;">  <h6 style="margin-top: 10px !important;">Research Study title &amp; Researcher Name</h6>  <p class="abt_p_margin" style="font-weight: 600;">Service Evaluation of Lynch Syndrome Patient Support App for mobile phonesThis research is being organised by Professor Julian Barwell FRCP(UK) PhD, Consultant Clinical Geneticist and Honorary Professor in Genomic Medicine at the University of Leicester.This Privacy Notice provides information about how the University of Leicester collects and uses your personal information when you take part in this research projects.</p>  <p class="abt_p_margin" style="font-weight: 600;">Please also refer to the Participant Information Sheet given to you for further details about the research project, what information will be collected about you, and how it will be used.</p>  <p class="abt_p_margin" style="font-weight: 600;">The University of Leicester will usually be the Data Controller of any data that you supply for this research. This means that we are responsible for looking after your information and using it properly. This means that the University will make the decisions on how your data is used and for what reasons. The exception to this is joint research projects, if this is applicable you will be informed on the Participant Information Sheet as to the other partner institution(s) who will also have responsibilities for looking after your information. You can access more information on this via the University&rsquo;s Information Assurance Services:</p>  <p style="font-weight: 600;">Information Assurance Services</p>  <p style="font-weight: 600;">University of Leicester</p>  <p style="font-weight: 600;">University Road</p>  <p style="font-weight: 600;">Leicester</p>  <p style="font-weight: 600;">LE1 7RH</p>  <p style="font-weight: 600;">T: +44 (0)116 229 7945</p>  <p style="font-weight: 600;">E: <a href="mailto:ias@le.ac.uk" target="_blank" rel="noopener">ias@le.ac.uk</a></p>  <p style="font-weight: 600;">W: <a href="https://www2.le.ac.uk/offices/ias" target="_blank" rel="noopener">https://www2.le.ac.uk/offices/ias</a></p>  <h6 style="margin-top: 10px !important;">Why do we need your data?</h6>  <p class="abt_p_margin" style="font-weight: 600;">We believe carriers of rare conditions often struggle to access timely information advice and that Apps like this Lynch Syndrome App can be helpful to patients to participate directly to prevent complications and personalise their health and well-being plans.</p>  <p class="abt_p_margin" style="font-weight: 600;">If, in this project, the App is shown to be useful to Lynch Syndrome carriers then the App may be offered to all Lynch Syndrome carriers in the UK.</p>  <h6>University of Leicester&rsquo;s legal basis for collecting this data is:</h6>  <p class="abt_p_margin" style="font-weight: 600;">Processing is necessary for the performance of a task in the public interest such as research.</p>  <p class="abt_p_margin" style="font-weight: 600;">If the university asks you for sensitive data such as data concerning health or genetic data, the University of Leicester will use these data because:</p>  <p class="abt_p_margin" style="font-weight: 600;">Processing is necessary for scientific or research into the usefulness of the Lynch Syndrome App to Lynch Syndrome carriers.</p>  <h6>What type of data will the University of Leicester use?</h6>  <p class="abt_p_margin" style="font-weight: 600;">We will collect the following mandatory data:</p>  <ul>  <li>Gender</li>  <li>Year of birth</li>  </ul>  <p class="abt_p_margin" style="font-weight: 600;">We will collect the following optional data which you may choose to enter in the Lynch Syndrome app or not:</p>  <ul>  <li>Date of diagnosis of Lynch Syndrome</li>  <li>How was Lynch Syndrome diagnosed</li>  <li>Name of any and all cancers diagnosed and date of diagnosis</li>  <li>Genetic Type of Lynch Syndrome</li>  <li>Are you aware of the symptoms to look out for</li>  <li>Have you discussed reproductive options with your GP</li>  <li>Information about a Helicobacter test and any result</li>  <li>Information about your taking aspirin therapy</li>  <li>Dates and results of any colonoscopies</li>  <li>If female and age &gt; 38, information about a Gynaecological review with GP</li>  <li>Information about notifying close family members about Lynch Syndrome</li>  </ul>  <h6>Who will the University of Leicester share your data with?</h6>  <p class="abt_p_margin" style="font-weight: 600;">The Lynch Syndrome App has been developed in collaboration with Instant Access Medical who have produced electronic patient records for participants in the Special Olympics around the world. With your consent Instant Access Medical will store the data securely with Amazon Web Services (AWS) in the UK.</p>  <p class="abt_p_margin" style="font-weight: 600;">The data collected as part of this study may be used, in part or in whole, to assess whether the App is useful to patients with Lynch Syndrome.</p>  <p class="abt_p_margin" style="font-weight: 600;">At no time will any personally identifiable data be published without your consent.Subject to your consent, your anonymised data will be evaluated by the NHS Genetic Medicine Service Alliance to assess how helpful and useful the App has been for Lynch Syndrome carriers. If it has been helpful and useful it may be offered to all Lynch Syndrome carriers in the UK.</p>  <h6>Will the University of Leicester transfer my data outside of the UK?</h6>  <p class="abt_p_margin" style="font-weight: 600;">Your data will not be transferred outside of the UK.</p>  <h6>What rights do I have regarding my data held by the University of Leicester?</h6>  <p class="abt_p_margin" style="font-weight: 600;">Your normal rights under the Data Protection Act and the General Data Protection Regulation apply. However, we need to manage your records in specific ways for the research project to be reliable. This means that we will not [always] be able to let you see or change the data we hold about you.</p>  <p class="abt_p_margin" style="font-weight: 600;">You can stop being part of the research project at any time, without giving a reason, but we will keep information about you that we already have and continue to use this for the purposes of the research project as outlined in the Participant Information Sheet.</p>  <h6>Where did the University of Leicester source my data from?</h6>  <p class="abt_p_margin" style="font-weight: 600;">Only you will be able to enter data into the Lynch Syndrome App. You may use a false name if you wish and not complete information that you do not wish to.We do ask that you enter an accurate gender and year of birth since these two factors determine the actions you are recommended to take to reduce your risk of cancer and encourage early detection of any cancer.</p>  <p class="abt_p_margin" style="font-weight: 600;">Any data you enter into the App will be known only to you unless you consent for it to be shared and even then it will be anonymised so that you are not personally identifiable.None of the data you may enter will be sent to your doctors or to their medical record systems. No data will be used or shared without your explicit consent.</p>  <h6>Are there any consequences of not providing the requested data?</h6>  <p class="abt_p_margin" style="font-weight: 600;">There are no consequences of not providing data for this research. It is purely voluntary.</p>  <h6>Will there be any automated decision making using my data?</h6>  <p class="abt_p_margin" style="font-weight: 600;">There will be no use of automated decision making in scope of UK Data Protection and Privacy legislation</p>  <h6>How long will the University of Leicester keep my data?</h6>  <p class="abt_p_margin" style="font-weight: 600;">In line with the law, we will only keep your data for as long as we need to so that we can fulfil our research objectives.</p>  <p class="abt_p_margin" style="font-weight: 600;">We will keep your personal data until we have completed all the actions that require us to hold them, for example sending you a copy of the results of the study if you have requested this, and then the data will be destroyed. This will take no longer than six months from when the study ends.</p>  <h6>Who can I contact if I have concerns?</h6>  <p class="abt_p_margin" style="font-weight: 600;">In the event of any questions about the research project, please contact the researchers in the first instance.</p>  <p class="abt_p_margin" style="font-weight: 600;">If you want to talk about this project or if you have a concern about any aspect of this study, please contact:</p>  <p class="abt_p_margin" style="font-weight: 600;">Prof. Julian Barwell FRCP PhD</p>  <p style="font-weight: 600;">Consultant in Clinical Genetics/Honorary Professor in Genetic Medicine</p>  <p style="font-weight: 600;">Clinical Genetics, Leicester Royal Infirmary, Leicester, LE1 5WW</p>  <p style="font-weight: 600;">0116 258 6042 (direct line)</p>  <p style="font-weight: 600;">0116 258 5736 (department)</p>  <p style="font-weight: 600;">Email: <a href="mailto:jgb8@le.ac.uk" target="_blank" rel="noopener">jgb8@le.ac.uk</a></p>  <p class="abt_p_margin" style="font-weight: 600;"><a href="https://le.ac.uk/people/julian-barwell" target="_blank" rel="noopener">https://le.ac.uk/people/julian-barwell</a></p>  <p class="abt_p_margin" style="font-weight: 600;">If you have any concerns about the way in which the research project has been conducted, please contact the Chair of the University Research Ethics Committee at <a href="mailto:ethics@leicester.ac.uk" target="_blank" rel="noopener">ethics@leicester.ac.uk</a>.</p>  <p class="abt_p_margin" style="font-weight: 600;">The University of Leicester Data Protection Officer is:</p>  <p style="font-weight: 600;">Data Protection Officer</p>  <p style="font-weight: 600;">University of Leicester</p>  <p style="font-weight: 600;">University Road, Leicester, LE1 7RH</p>  <p style="font-weight: 600;">0116 229 7640</p>  <p style="font-weight: 600;"><a href="mailto:dpo@le.ac" target="_blank" rel="noopener">DPO@le.ac.uk</a></p>  <p class="abt_p_margin" style="font-weight: 600;">For further details about information security, please contact the Information Assurance Services team:</p>  <p style="font-weight: 600;">Information Assurance Services</p>  <p style="font-weight: 600;">University of Leicester</p>  <p style="font-weight: 600;">University Road</p>  <p style="font-weight: 600;">Leicester</p>  <p style="font-weight: 600;">LE1 7RH</p>  <p style="font-weight: 600;">T: +44 (0)116 229 7945</p>  <p style="font-weight: 600;">E: <a href="mailto:dpo@le.ac" target="_blank" rel="noopener">dpo@le.ac</a></p>  <p style="font-weight: 600;">W: <a href="https://www2.le.ac.uk/offices/ias" target="_blank" rel="noopener">https://www2.le.ac.uk/offices/ias</a></p>  </div>  </div>  </div>',1,20,0,0,1,GETDATE(),'PPL',1,0)