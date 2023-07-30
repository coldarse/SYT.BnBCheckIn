import { BnBCheckInTemplatePage } from './app.po';

describe('BnBCheckIn App', function() {
  let page: BnBCheckInTemplatePage;

  beforeEach(() => {
    page = new BnBCheckInTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
