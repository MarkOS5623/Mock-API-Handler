using BugTriageWorkflow.Models;

namespace BugTriageWorkflow.Data;

/// <summary>
/// Provides grouped sample bug reports used to test the workflow with different training data sets.
/// Each case contains several related reports with different evidence quality.
/// </summary>
public static class SampleBugReports {
    public static Dictionary<string, List<BugReport>> CreateCases() {
        return new Dictionary<string, List<BugReport>> {
            ["Evidence Supports Bug"] = [
                new BugReport {
                    Id = "BUG-001",
                    Reporter = "qa-user-01",
                    RawText = """
                              A customer reported that the checkout page becomes unresponsive near the end of the purchase flow.
                              The issue happens after payment details are entered and the user clicks the checkout button.
                              The screen remains visible, but the button no longer reacts and the order is not completed.
                              Several users reported that they had to refresh the browser and restart the purchase.
                              """,
                    Evidence = """
                               Automated UI test failed during the checkout submission step.
                               Browser console shows a timeout error after clicking the checkout button.
                               Checkout API returned 504 Gateway Timeout.
                               The payment form was valid, but the request did not finish successfully.
                               """
                },
                new BugReport {
                    Id = "BUG-002",
                    Reporter = "qa-user-02",
                    RawText = """
                              Users can sign in successfully, but the orders page does not show recent orders.
                              The page header loads, navigation remains visible, and no loading spinner is displayed.
                              The main content area is blank after login.
                              Users cannot confirm whether their previous purchases exist.
                              """,
                    Evidence = """
                               Frontend error monitoring captured a React rendering exception.
                               The same issue reproduced in Chrome and Edge.
                               Screenshot evidence shows an empty orders page.
                               API responses for orders returned 200, but the client failed while rendering the result.
                               """
                },
                new BugReport {
                    Id = "BUG-003",
                    Reporter = "qa-user-03",
                    RawText = """
                              Users who forgot their password cannot complete account recovery.
                              The password reset form accepts the email address and shows a success message.
                              However, users report that the reset email never arrives.
                              This blocks users from regaining access to their accounts.
                              """,
                    Evidence = """
                               Mail service logs show failed SMTP delivery.
                               Backend logs contain email provider timeout errors.
                               Password reset requests are recorded but no email is delivered.
                               Several queued email jobs retried and failed with provider timeout messages.
                               """
                },
                new BugReport {
                    Id = "BUG-004",
                    Reporter = "qa-user-10",
                    RawText = """
                              The admin dashboard intermittently fails when loading user statistics.
                              The page opens normally, but the statistics panel displays an error placeholder.
                              Admins can still use the sidebar and other pages.
                              The failure appears when the dashboard requests aggregated user activity data.
                              """,
                    Evidence = """
                               Backend logs show repeated database deadlock errors during the statistics aggregation query.
                               The dashboard API returned 500 for the statistics endpoint.
                               Monitoring confirms failures started after the latest reporting query change.
                               Other dashboard endpoints remained healthy.
                               """
                },
                new BugReport {
                    Id = "BUG-005",
                    Reporter = "qa-user-11",
                    RawText = """
                              Mobile users report that uploaded profile images appear broken after saving.
                              The upload screen shows success and the profile page refreshes.
                              Instead of the new image, users see a broken image icon.
                              The issue appears on both Android and iOS test devices.
                              """,
                    Evidence = """
                               Image processing logs show failed thumbnail generation.
                               Storage upload completed successfully, but CDN image transformation returned 502.
                               Screenshots from both mobile platforms show broken image placeholders.
                               The original image file exists in storage, but generated preview URLs fail.
                               """
                }
            ],

            ["Evidence Contradicts Bug"] = [
                new BugReport {
                    Id = "BUG-006",
                    Reporter = "qa-user-04",
                    RawText = """
                              A user reported that the login button does not respond when clicked.
                              They claimed the application cannot be accessed from the login screen.
                              The report says the button stays idle and no navigation occurs.
                              No browser, device, or reproduction details were included.
                              """,
                    Evidence = """
                               Automated UI test passed successfully.
                               The login button was clicked and navigation completed.
                               Screenshot comparison shows the login flow behaved as expected.
                               Authentication logs show successful login for the same test account during the reported window.
                               """
                },
                new BugReport {
                    Id = "BUG-007",
                    Reporter = "qa-user-05",
                    RawText = """
                              The search box allegedly returns no results for any query.
                              The report claims search is completely broken across the whole application.
                              The reporter did not include screenshots or query examples.
                              The issue was described as affecting all users.
                              """,
                    Evidence = """
                               Automated search tests passed for multiple queries.
                               Logs show successful 200 responses from the search API.
                               Monitoring shows normal search traffic and normal result counts.
                               Product analytics show users continued clicking search results during the reported period.
                               """
                },
                new BugReport {
                    Id = "BUG-008",
                    Reporter = "qa-user-06",
                    RawText = """
                              The profile save button reportedly does not save changes.
                              The report says user profile updates are lost after pressing save.
                              The reporter claims refreshing the page restores the old profile data.
                              No console errors or screenshots were attached.
                              """,
                    Evidence = """
                               Database audit logs show profile updates were saved successfully.
                               API returned 200 OK for the profile update request.
                               UI test confirmed the updated profile fields persisted after refresh.
                               The saved values match the payload submitted by the test user.
                               """
                },
                new BugReport {
                    Id = "BUG-009",
                    Reporter = "qa-user-12",
                    RawText = """
                              A report claims that invoice PDF downloads are failing for all customers.
                              The user says clicking the download button does nothing.
                              They also claim there is no network request when attempting the download.
                              The report does not mention invoice id, browser, or account type.
                              """,
                    Evidence = """
                               Download monitoring shows normal invoice PDF download volume.
                               API logs show 200 responses for invoice download requests.
                               Browser automation successfully downloaded PDFs for multiple invoice ids.
                               CDN access logs confirm files were served during the reported time period.
                               """
                },
                new BugReport {
                    Id = "BUG-010",
                    Reporter = "qa-user-13",
                    RawText = """
                              A report states that the notification settings page cannot be opened.
                              The reporter says the page crashes immediately after clicking settings.
                              They claim the issue affects every browser.
                              No stack trace or screenshot was provided.
                              """,
                    Evidence = """
                               Frontend monitoring shows no crash events for the settings page.
                               Automated navigation tests opened notification settings successfully.
                               Browser compatibility tests passed in Chrome, Edge, and Firefox.
                               Session replay for the reporter shows the settings page loaded without errors.
                               """
                }
            ],

            ["Evidence Mixed Or Inconclusive"] = [
                new BugReport {
                    Id = "BUG-011",
                    Reporter = "qa-user-07",
                    RawText = """
                              Search results appear to load slowly for some users.
                              The report says users sometimes wait more than 10 seconds.
                              The issue is not constant and appears to happen more often during peak hours.
                              Refreshing the page sometimes improves the experience.
                              """,
                    Evidence = """
                               Monitoring shows average search response time is normal.
                               However, logs show occasional timeout spikes for some users.
                               No consistent reproduction steps were found.
                               Regional latency metrics show brief spikes but not enough to confirm a widespread outage.
                               """
                },
                new BugReport {
                    Id = "BUG-012",
                    Reporter = "qa-user-08",
                    RawText = """
                              Notifications sometimes do not appear after new messages arrive.
                              Users may miss important updates if the notification is delayed.
                              The issue appears intermittent and does not happen for every message.
                              The reporter saw it once during testing but could not reproduce it consistently.
                              """,
                    Evidence = """
                               Push notification service reports normal delivery rates.
                               Some client logs show delayed websocket reconnects.
                               The issue was reproduced once but not consistently.
                               Server logs confirm message creation, but client-side timing varies across sessions.
                               """
                },
                new BugReport {
                    Id = "BUG-013",
                    Reporter = "qa-user-09",
                    RawText = """
                              Dashboard charts occasionally show outdated data.
                              Refreshing the page sometimes fixes the problem.
                              The reporter believes stale data may be cached somewhere in the client.
                              The issue was seen only during one browser session.
                              """,
                    Evidence = """
                               Cache logs show some stale responses.
                               Database query results are current.
                               QA could reproduce the issue only on one browser session.
                               Browser cache clearing fixed the issue, but the root cause was not confirmed.
                               """
                },
                new BugReport {
                    Id = "BUG-014",
                    Reporter = "qa-user-14",
                    RawText = """
                              Some users say file uploads are slow and occasionally fail.
                              The failures seem more common for larger files.
                              The upload progress bar sometimes pauses before finishing.
                              Smaller uploads usually complete without problems.
                              """,
                    Evidence = """
                               Storage provider metrics show normal availability.
                               Application logs show a small number of upload timeout warnings.
                               Network traces from one test run show packet loss.
                               Other test runs completed successfully with the same file size.
                               """
                },
                new BugReport {
                    Id = "BUG-015",
                    Reporter = "qa-user-15",
                    RawText = """
                              The reports export page sometimes generates CSV files with missing rows.
                              The issue was reported after filtering by date range.
                              Export works correctly for smaller date ranges.
                              The problem may be related to large reports or pagination.
                              """,
                    Evidence = """
                               Export service logs show successful job completion.
                               QA found one CSV with fewer rows than expected.
                               Database query counts differ depending on pagination settings.
                               The issue could not be reproduced consistently across multiple date ranges.
                               """
                }
            ]
        };
    }
}